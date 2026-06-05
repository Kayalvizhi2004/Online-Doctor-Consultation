using ConsultationApi.Domain.Common;

namespace ConsultationApi.Domain.Entities.Doctors;

public class AvailabilitySlot : BaseEntity
{
    public Guid DoctorId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsBooked { get; set; }

    public DoctorProfile Doctor { get; set; } = null!;
}