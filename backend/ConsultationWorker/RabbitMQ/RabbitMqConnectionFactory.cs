using Microsoft.Extensions.Options;
using ConsultationWorker.Configurations;
using RabbitMQ.Client;

namespace ConsultationWorker.RabbitMQ;

/// <summary>
/// Factory for creating RabbitMQ connections using RabbitMQ.Client v6.8.1.
/// Uses proper interface types instead of reflection/dynamic.
/// </summary>
public class RabbitMqConnectionFactory
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqConnectionFactory(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
    }

    /// <summary>
    /// Creates a new RabbitMQ connection with proper interface typing.
    /// </summary>
    /// <returns>IConnection instance</returns>
    public IConnection CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create RabbitMQ connection to {_settings.HostName}:{_settings.Port}. " +
                "Ensure RabbitMQ is running and credentials are correct.", ex);
        }
    }
}