using System.Text;
using System.Text.Json;
using ConsultationApi.Infrastructure.RabbitMQ.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConsultationApi.Domain.Interfaces.Messaging;
using RabbitMQ.Client;

namespace ConsultationApi.Infrastructure.RabbitMQ.Publishers;

public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public void Publish<T>(T message, string routingKey)
    {
        var payload = JsonSerializer.Serialize(message);

        try
        {
            var factoryType = Type.GetType("RabbitMQ.Client.ConnectionFactory, RabbitMQ.Client");
            if (factoryType == null)
                throw new InvalidOperationException("RabbitMQ.Client.ConnectionFactory type not found. Is RabbitMQ.Client available at runtime?");

            dynamic factory = Activator.CreateInstance(factoryType)!;
            factory.HostName = _settings.HostName;
            factory.UserName = _settings.UserName;
            factory.Password = _settings.Password;

            dynamic connection = factory.CreateConnection();
            dynamic channel = connection.CreateModel();

                // Declare exchange (idempotent)
                channel.ExchangeDeclare(
                    exchange: _settings.ExchangeName,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false);

                // Publish message
                var body = Encoding.UTF8.GetBytes(payload);
                var props = channel.CreateBasicProperties();
                props.Persistent = true;

                channel.BasicPublish(
                    exchange: _settings.ExchangeName,
                    routingKey: routingKey,
                    basicProperties: props,
                    body: body);

            _logger.LogInformation("[RabbitMqPublisher] Published {RoutingKey} size={Size}", routingKey, body.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ for routingKey={RoutingKey}", routingKey);
            throw;
        }
    }

    public void Dispose()
    {
        // No resources to dispose, connection/channel are disposed in using statements
    }
}