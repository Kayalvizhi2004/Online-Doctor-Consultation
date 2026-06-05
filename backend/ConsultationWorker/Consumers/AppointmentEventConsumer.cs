using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConsultationWorker.RabbitMQ;
using ConsultationWorker.Services;
using ConsultationWorker.Configurations;
using RabbitMQ.Client;

namespace ConsultationWorker.Consumers;

/// <summary>
/// Background service that consumes appointment events from RabbitMQ.
/// Listens on 4 event queues and processes messages using NotificationProcessor.
/// Failed messages are automatically sent to the Dead Letter Queue.
/// </summary>
public class AppointmentEventConsumer : BackgroundService
{
    private readonly RabbitMqConnectionFactory _factory;
    private readonly NotificationProcessor _processor;
    private readonly ILogger<AppointmentEventConsumer> _logger;
    private readonly RabbitMqSettings _settings;

    private IConnection? _connection;
    private IModel? _channel;

    public AppointmentEventConsumer(
        RabbitMqConnectionFactory factory,
        NotificationProcessor processor,
        IOptions<RabbitMqSettings> settings,
        ILogger<AppointmentEventConsumer> logger)
    {
        _factory = factory;
        _processor = processor;
        _logger = logger;
        _settings = settings.Value;
    }

    /// <summary>
    /// Initializes RabbitMQ connection and declares all exchanges/queues.
    /// </summary>
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[AppointmentEventConsumer] Starting background service");

        try
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            if (_channel == null)
            {
                _logger.LogError("[AppointmentEventConsumer] Failed to create RabbitMQ channel");
                return Task.CompletedTask;
            }

            // Set Quality of Service (QoS) - process 1 message at a time
            _channel.BasicQos(0, 1, false);
            _logger.LogInformation("[AppointmentEventConsumer] QoS set to prefetch 1 message");

            // Ensure exchanges/queues exist at startup (idempotent operations)
            try
            {
                _logger.LogInformation("[AppointmentEventConsumer] Configuring RabbitMQ infrastructure");
                DeadLetterSetup.Configure(_channel);
                ExchangeSetup.Configure(_channel);
                QueueBindings.Configure(_channel);
                _logger.LogInformation("[AppointmentEventConsumer] RabbitMQ infrastructure configured successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AppointmentEventConsumer] Failed to configure RabbitMQ infrastructure");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AppointmentEventConsumer] Failed to initialize RabbitMQ connection");
            throw;
        }

        return base.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Polls 4 event queues continuously and processes each message.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            _logger.LogError("[AppointmentEventConsumer] AMQP channel not available, exiting");
            return;
        }

        var queues = new[]
        {
            "appointment.booked.queue",
            "appointment.confirmed.queue",
            "appointment.cancelled.queue",
            "consultation.completed.queue"
        };

        _logger.LogInformation(
            "[AppointmentEventConsumer] Started polling queues: {Queues}",
            string.Join(", ", queues));

        int consecutiveEmptyPolls = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                bool messageProcessed = false;

                foreach (var queueName in queues)
                {
                    try
                    {
                        // Try to get a message from the queue (non-blocking)
                        var result = _channel.BasicGet(queueName, autoAck: false);
                        if (result == null)
                            continue; // No message available

                        messageProcessed = true;
                        consecutiveEmptyPolls = 0;

                        var routingKey = result.RoutingKey ?? string.Empty;
                        var body = result.Body.ToArray();

                        _logger.LogInformation(
                            "[AppointmentEventConsumer] Received message from {Queue} with routing key '{RoutingKey}' (size={Size} bytes)",
                            queueName, routingKey, body.Length);

                        try
                        {
                            // Process the message
                            await _processor.ProcessAsync(routingKey, body, stoppingToken);

                            // Acknowledge successful processing
                            _channel.BasicAck(result.DeliveryTag, multiple: false);

                            _logger.LogInformation(
                                "[AppointmentEventConsumer] Successfully processed message with routing key '{RoutingKey}'",
                                routingKey);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "[AppointmentEventConsumer] Failed to process message from {Queue} with routing key '{RoutingKey}'",
                                queueName, routingKey);

                            // Negative acknowledgment sends message to DLQ
                            try
                            {
                                _channel.BasicNack(result.DeliveryTag, multiple: false, requeue: false);
                                _logger.LogInformation("[AppointmentEventConsumer] Message nacked and sent to DLQ for queue {Queue}", queueName);
                            }
                            catch (Exception nackEx)
                            {
                                _logger.LogError(nackEx, "[AppointmentEventConsumer] Failed to nack message");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[AppointmentEventConsumer] Error processing queue {Queue}", queueName);
                    }
                }

                // Adjust delay based on whether we found messages
                if (!messageProcessed)
                {
                    consecutiveEmptyPolls++;
                    if (consecutiveEmptyPolls % 10 == 0)
                    {
                        _logger.LogDebug("[AppointmentEventConsumer] No messages found in {ConsecutivePolls} consecutive polls", consecutiveEmptyPolls);
                    }
                    // Exponential backoff: wait longer if no messages found
                    await Task.Delay(Math.Min(500 + (consecutiveEmptyPolls * 50), 2000), stoppingToken);
                }
                else
                {
                    // Short delay after processing a message
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[AppointmentEventConsumer] Polling loop cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AppointmentEventConsumer] Error in polling loop");
                // Wait before retrying to avoid tight loop on errors
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    /// <summary>
    /// Gracefully closes RabbitMQ connection on shutdown.
    /// </summary>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[AppointmentEventConsumer] Stopping background service");

        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("[AppointmentEventConsumer] RabbitMQ connection closed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AppointmentEventConsumer] Error closing RabbitMQ connection");
        }

        return base.StopAsync(cancellationToken);
    }
}
