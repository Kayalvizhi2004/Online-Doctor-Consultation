namespace ConsultationApi.Domain.Events.Base;

public interface IDomainEvent
{
    Guid EventId { get; }

    DateTime OccurredAt { get; }
}