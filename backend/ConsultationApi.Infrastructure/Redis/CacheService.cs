using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ConsultationApi.Infrastructure.Redis;

public class CacheService : ConsultationApi.Domain.Interfaces.Redis.ICacheService
{
    private readonly IDatabase? _database;
    private readonly IConnectionMultiplexer?
        _redis;
    private readonly ILogger<CacheService>
        _logger;

    public CacheService(
        RedisConnectionFactory factory,
        ILogger<CacheService> logger)
    {
        _logger = logger;

        try
        {
            _redis = factory.Connection;
            _database = _redis.GetDatabase();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis is unavailable; operating with no cache.");
            _redis = null;
            _database = null;
        }
    }

    public async Task<T?> GetAsync<T>(
        string key)
    {
        if (_database == null)
            return default;

        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            _logger.LogDebug("CACHE MISS {Key}", key);
            return default;
        }

        _logger.LogDebug("CACHE HIT {Key}", key);
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan ttl)
    {
        if (_database == null)
            return;

        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, ttl);
        _logger.LogDebug("CACHE SET {Key}", key);
    }

    public async Task RemoveAsync(
        string key)
    {
        if (_database == null)
            return;

        await _database.KeyDeleteAsync(key);
        _logger.LogDebug("CACHE REMOVE {Key}", key);
    }

    public async Task RemoveByPatternAsync(
        string pattern)
    {
        if (_redis == null || _database == null)
            return;

        foreach (var endpoint in _redis.GetEndPoints())
        {
            var server = _redis.GetServer(endpoint);
            var keys = server.Keys(pattern: $"*{pattern}*");

            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }

        _logger.LogDebug("CACHE REMOVE PATTERN {Pattern}", pattern);
    }
}