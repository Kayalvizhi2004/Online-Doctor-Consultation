using StackExchange.Redis;

namespace ConsultationApi.Infrastructure.SignalR;

public class ConnectionTracker
{
    private readonly IConnectionMultiplexer
        _redis;

    public ConnectionTracker(
        IConnectionMultiplexer redis)
    {
        _redis = redis;
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
    }

    public async Task RemoveConnectionAsync(
        Guid sessionId,
        Guid userId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:connections:{sessionId}";

        await db.HashDeleteAsync(
            key,
            userId.ToString());
    }

    public async Task<string?>
        GetConnectionAsync(
            Guid sessionId,
            Guid userId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:connections:{sessionId}";

        var result =
            await db.HashGetAsync(
                key,
                userId.ToString());

        return result.HasValue
            ? result.ToString()
            : null;
    }

    public async Task<
        Dictionary<string, string>>
        GetSessionConnectionsAsync(
            Guid sessionId)
    {
        var db = _redis.GetDatabase();

        var key =
            $"session:connections:{sessionId}";

        var entries =
            await db.HashGetAllAsync(
                key);

        return entries.ToDictionary(
            x => x.Name.ToString(),
            x => x.Value.ToString());
    }
}