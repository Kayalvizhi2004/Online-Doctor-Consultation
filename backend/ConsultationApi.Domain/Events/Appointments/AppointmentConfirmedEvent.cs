using ConsultationApi.Domain.Events.Base;

namespace ConsultationApi.Domain.Events.Appointments;

public class AppointmentConfirmedEvent
    : IDomainEvent
{
    public Guid EventId { get; init; }

    public Guid AppointmentId { get; init; }

    public Guid PatientId { get; init; }

    public Guid DoctorId { get; init; }

    public DateTime OccurredAt { get; init; }
}