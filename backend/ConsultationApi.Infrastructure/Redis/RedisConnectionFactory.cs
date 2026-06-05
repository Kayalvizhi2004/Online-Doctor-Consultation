using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace ConsultationApi.Infrastructure.Redis;

public class RedisConnectionFactory
{
    private readonly Lazy<IConnectionMultiplexer>
        _connection;

    public RedisConnectionFactory(
        IConfiguration configuration)
    {
        var connectionString =
            configuration["Redis:ConnectionString"]
            ?? configuration.GetSection("Redis")?["ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Redis connection string is not configured. Set 'Redis:ConnectionString' in configuration.");

        try
        {
            var options = StackExchange.Redis.ConfigurationOptions.Parse(connectionString);
            // Allow the multiplexer to keep retrying rather than throwing on startup
            options.AbortOnConnectFail = false;

            _connection = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }
        catch (Exception)
        {
            // Fallback: try connecting directly (keeps previous behavior if Parse isn't applicable)
            _connection = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
        }
    }

    public IConnectionMultiplexer
        Connection
            => _connection.Value;
}