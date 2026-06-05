using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ConsultationWorker.RabbitMQ;
using ConsultationWorker.Services;
using RabbitMQ.Client;

namespace ConsultationWorker.RabbitMQ;

public class AppointmentEventConsumer : BackgroundService
{
    private readonly ILogger<AppointmentEventConsumer> _logger;
    private readonly RabbitMqConnectionFactory _factory;
    private readonly NotificationProcessor _processor;

    private IConnection? _connection;
    private IModel? _channel;

    public AppointmentEventConsumer(
        RabbitMqConnectionFactory factory,
        NotificationProcessor processor,
        ILogger<AppointmentEventConsumer> logger)
    {
        _factory = factory;
        _processor = processor;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[AppointmentEventConsumer] Starting");

        try
        {
            // Create connection
            _connection = _factory.CreateConnection();
            
            // Create channel using sync CreateModel for RabbitMQ.Client 6.8.1
            _channel = _connection.CreateModel();

            // Ensure exchanges/queues exist
            DeadLetterSetup.Configure(_channel);
            ExchangeSetup.Configure(_channel);
            QueueBindings.Configure(_channel);

            _logger.LogInformation("[AppointmentEventConsumer] RabbitMQ infrastructure initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AppointmentEventConsumer] Failed to initialize RabbitMQ");
            throw;
        }

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            _logger.LogError("[AppointmentEventConsumer] Channel is null, cannot start polling");
            return;
        }

        var queues = new[]
        {
            "appointment.booked.queue",
            "appointment.confirmed.queue",
            "appointment.cancelled.queue",
            "consultation.completed.queue"
        };

        _logger.LogInformation("[AppointmentEventConsumer] Started polling {QueueCount} queues", queues.Length);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var queueName in queues)
                {
                    try
                    {
                        var result = _channel.BasicGet(queueName, autoAck: false);
                        if (result == null)
                            continue;

                        string routingKey = result.RoutingKey ?? string.Empty;
                        byte[] body = result.Body.ToArray();
                        ulong deliveryTag = result.DeliveryTag;

                        try
                        {
                            _logger.LogInformation("[AppointmentEventConsumer] Processing message from {Queue} with routing key: {RoutingKey}", queueName, routingKey);
                            await _processor.ProcessAsync(routingKey, body, stoppingToken);
                            _channel.BasicAck(deliveryTag, multiple: false);
                            _logger.LogInformation("[AppointmentEventConsumer] Message acknowledged for {Queue}", queueName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[AppointmentEventConsumer] Error processing message from {Queue}, NACKing", queueName);
                            _channel.BasicNack(deliveryTag, multiple: false, requeue: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[AppointmentEventConsumer] Error reading from queue {Queue}", queueName);
                    }
                }

                await Task.Delay(500, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[AppointmentEventConsumer] Cancellation requested, stopping polling");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AppointmentEventConsumer] Unexpected error in polling loop, will continue");
                await Task.Delay(1000, stoppingToken);
            }
        }

        _logger.LogInformation("[AppointmentEventConsumer] Polling loop ended");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[AppointmentEventConsumer] Stopping");

        try
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AppointmentEventConsumer] Error disposing connection/channel");
        }

        await base.StopAsync(cancellationToken);
    }
}
