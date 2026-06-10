using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConsultationWorker.RabbitMQ;
using RabbitMQ.Client.Events;
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
    /// Registers consumers for each queue to listen for incoming messages.
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
            "consultation.started.queue",
            "consultation.completed.queue"
        };

        foreach (var queueName in queues)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var routingKey = ea.RoutingKey;

                _logger.LogInformation("[AppointmentEventConsumer] Received message from {Queue}", queueName);

                try
                {
                    await _processor.ProcessAsync(routingKey, body, stoppingToken);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AppointmentEventConsumer] Error processing message. Nacking to DLQ.");
                    // Nack and do NOT requeue (sends to Dead Letter Queue)
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("[AppointmentEventConsumer] Subscribed to queue: {Queue}", queueName);
        }

        try
        {
            // Keep the service alive while consumers are listening for events
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("[AppointmentEventConsumer] Background service stopping...");
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

