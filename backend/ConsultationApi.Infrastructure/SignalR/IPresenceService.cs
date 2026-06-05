using System;

namespace ConsultationApi.Infrastructure.SignalR;

public interface IPresenceService
{
    Task AddConnectionAsync(Guid sessionId, Guid userId, string connectionId);

    Task RemoveConnectionAsync(Guid sessionId, string connectionId);

    Task RemoveConnectionAsync(string connectionId);

    Task SetSessionActiveAsync(Guid sessionId);

    Task<bool> IsSessionActiveAsync(Guid sessionId);

    Task RefreshSessionAsync(Guid sessionId);

    Task EndSessionAsync(Guid sessionId);
}
