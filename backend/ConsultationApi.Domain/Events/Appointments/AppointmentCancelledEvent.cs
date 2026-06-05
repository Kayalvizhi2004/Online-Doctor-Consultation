using ConsultationApi.Domain.Events.Base;

namespace ConsultationApi.Domain.Events.Appointments;

public class AppointmentCancelledEvent
    : IDomainEvent
{
    public Guid EventId { get; init; }

    public Guid AppointmentId { get; init; }

    public Guid PatientId { get; init; }

    public Guid DoctorId { get; init; }

    public string Reason { get; init; }
        = string.Empty;

    public DateTime OccurredAt { get; init; }
}