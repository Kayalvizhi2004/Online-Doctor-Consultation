using ConsultationApi.Domain.Events.Base;

namespace ConsultationApi.Domain.Events.Notifications;

public class NotificationCreatedEvent
    : IDomainEvent
{
    public Guid EventId { get; init; }

    public Guid UserId { get; init; }

    public string Title { get; init; }
        = string.Empty;

    public string Message { get; init; }
        = string.Empty;

    public DateTime OccurredAt { get; init; }
}