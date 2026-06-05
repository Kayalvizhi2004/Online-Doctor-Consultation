using ConsultationApi.Domain.Common;

namespace ConsultationApi.Domain.Entities.Appointments;

public class ConsultationSession : BaseEntity
{
    public Guid AppointmentId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public string? Summary { get; set; }

    public Appointment Appointment { get; set; } = null!;
}