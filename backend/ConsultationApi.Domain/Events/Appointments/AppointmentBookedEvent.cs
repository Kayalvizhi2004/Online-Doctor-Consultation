using ConsultationApi.Domain.Events.Base;

namespace ConsultationApi.Domain.Events.Appointments;

public class AppointmentBookedEvent
    : IDomainEvent
{
    public Guid EventId { get; init; }

    public Guid AppointmentId { get; init; }

    public Guid PatientId { get; init; }

    public string PatientName { get; init; }
        = string.Empty;

    public Guid DoctorId { get; init; }

    public DateOnly SlotDate { get; init; }

    public string SlotTime { get; init; }
        = string.Empty;

    public DateTime OccurredAt { get; init; }
}