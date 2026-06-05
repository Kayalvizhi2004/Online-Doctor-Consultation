using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConsultationWorker.RabbitMQ;
using ConsultationWorker.Services;
using ConsultationWorker.Configurations;

namespace ConsultationWorker.Consumers;

public class AppointmentEventConsumer : BackgroundService
{
    private readonly RabbitMqConnectionFactory _factory;
    private readonly NotificationProcessor _processor;
    private readonly ILogger<AppointmentEventConsumer> _logger;
    private readonly RabbitMqSettings _settings;

    private dynamic? _connection;
    private dynamic? _channel;

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

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("AppointmentEventConsumer starting");

        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Ensure exchanges/queues exist at startup (worker declares them)
        try
        {
            DeadLetterSetup.Configure(_channel);
            ExchangeSetup.Configure(_channel);
            QueueBindings.Configure(_channel);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to configure exchange/queues at startup");
        }

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
        {
            _logger.LogError("AMQP channel not available, exiting consumer");
            return;
        }

        var queues = new[]
        {
            _settings.AppointmentQueue ?? "appointment.booked.queue",
            "appointment.confirmed.queue",
            "appointment.cancelled.queue",
            "consultation.completed.queue"
        };

        _logger.LogInformation("AppointmentEventConsumer polling queues: {Queues}", string.Join(',', queues));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var q in queues)
                {
                    var result = _channel.BasicGet(q, false);
                    if (result == null)
                        continue;

                    var routingKey = result.RoutingKey as string ?? string.Empty;
                    var body = ((ReadOnlyMemory<byte>)result.Body).ToArray();

                    try
                    {
                        await _processor.ProcessAsync(routingKey, body, stoppingToken);
                        _channel.BasicAck(result.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed processing message from {Queue}", q);
                        try { _channel.BasicNack(result.DeliveryTag, false, false); } catch { }
                    }
                }

                await Task.Delay(500, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in polling loop");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("AppointmentEventConsumer stopping");

        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch { }

        return base.StopAsync(cancellationToken);
    }
}
