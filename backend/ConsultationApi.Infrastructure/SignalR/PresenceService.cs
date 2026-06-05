using StackExchange.Redis;

namespace ConsultationApi.Infrastructure.SignalR;

public class PresenceService : IPresenceService
{
    private readonly IConnectionMultiplexer
        _redis;

    public PresenceService(
        IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetSessionActiveAsync(
        Guid sessionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:active:{sessionId}";

        await db.StringSetAsync(
            key,
            "active",
            TimeSpan.FromMinutes(30));
    }

    public async Task<bool>
        IsSessionActiveAsync(
            Guid sessionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:active:{sessionId}";

        return await db.KeyExistsAsync(
            key);
    }

    public async Task RefreshSessionAsync(
        Guid sessionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:active:{sessionId}";

        await db.KeyExpireAsync(
            key,
            TimeSpan.FromMinutes(30));
    }

    public async Task EndSessionAsync(
        Guid sessionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:active:{sessionId}";

        await db.KeyDeleteAsync(
            key);
    }

    public async Task AddConnectionAsync(
        Guid sessionId,
        Guid userId,
        string connectionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:connections:{sessionId}";

        await db.HashSetAsync(
            key,
            userId.ToString(),
            connectionId);

        await db.KeyExpireAsync(
            key,
            TimeSpan.FromMinutes(30));

        await SetSessionActiveAsync(sessionId);
    }

    public async Task RemoveConnectionAsync(
        Guid sessionId,
        string connectionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:connections:{sessionId}";

        var entries =
            await db.HashGetAllAsync(key);

        foreach (var entry in entries)
        {
            if (entry.Value == connectionId)
            {
                await db.HashDeleteAsync(
                    key,
                    entry.Name);
                break;
            }
        }
    }

    public async Task RemoveConnectionAsync(
        string connectionId)
    {
        var endpoints = _redis.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);

            var keys = server.Keys(
                pattern: "session:connections:*");

            foreach (var key in keys)
            {
                var db = _redis.GetDatabase();

                var entries =
                    await db.HashGetAllAsync(key);

                foreach (var entry in entries)
                {
                    if (entry.Value == connectionId)
                    {
                        await db.HashDeleteAsync(
                            key,
                            entry.Name);
                    }
                }
            }
        }
    }
}