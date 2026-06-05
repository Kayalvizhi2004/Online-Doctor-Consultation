using System.Text;
using System.Text.Json;
using ConsultationApi.Infrastructure.RabbitMQ.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ConsultationApi.Domain.Interfaces.Messaging;
using RabbitMQ.Client;

namespace ConsultationApi.Infrastructure.RabbitMQ.Publishers;

/// <summary>
/// Publishes appointment events to RabbitMQ using RabbitMQ.Client v6.8.1.
/// Uses proper interface types for type safety.
/// </summary>
public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lockObject = new();

    public RabbitMqPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Publishes a message to the configured exchange with a routing key.
    /// Ensures exchange exists and message is persisted.
    /// </summary>
    /// <typeparam name="T">Message type (must be JSON serializable)</typeparam>
    /// <param name="message">Message payload</param>
    /// <param name="routingKey">RabbitMQ routing key</param>
    public void Publish<T>(T message, string routingKey)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        if (string.IsNullOrWhiteSpace(routingKey))
            throw new ArgumentException("Routing key cannot be null or empty", nameof(routingKey));

        var payload = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(payload);

        try
        {
            // Get or create channel (thread-safe)
            var channel = GetOrCreateChannel();

            // Declare exchange (idempotent - safe to call multiple times)
            channel.ExchangeDeclare(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            // Create persistent message properties
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            // Publish message
            channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                basicProperties: props,
                body: body);

            _logger.LogInformation(
                "[RabbitMqPublisher] Published event with routing key '{RoutingKey}' (size={Size} bytes)",
                routingKey, body.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[RabbitMqPublisher] Failed to publish message with routing key '{RoutingKey}'",
                routingKey);
            throw;
        }
    }

    /// <summary>
    /// Gets existing channel or creates new connection/channel if needed.
    /// Thread-safe using lock.
    /// </summary>
    private IModel GetOrCreateChannel()
    {
        if (_channel != null && _channel.IsOpen)
            return _channel;

        lock (_lockObject)
        {
            // Double-check after acquiring lock
            if (_channel != null && _channel.IsOpen)
                return _channel;

            try
            {
                // Close old connection if exists
                _connection?.Dispose();

                // Create new connection
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    UserName = _settings.UserName,
                    Password = _settings.Password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation(
                    "[RabbitMqPublisher] Created new RabbitMQ connection to {HostName}",
                    _settings.HostName);

                return _channel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[RabbitMqPublisher] Failed to create RabbitMQ connection to {HostName}. " +
                    "Verify RabbitMQ is running and credentials are correct.",
                    _settings.HostName);
                throw;
            }
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[RabbitMqPublisher] Error during disposal");
        }
    }
}