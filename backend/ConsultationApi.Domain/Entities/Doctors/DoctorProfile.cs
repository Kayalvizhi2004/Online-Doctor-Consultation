using ConsultationApi.Domain.Common;
using ConsultationApi.Domain.Entities.Users;

namespace ConsultationApi.Domain.Entities.Doctors;

public class DoctorProfile : BaseEntity
{
    public Guid UserId { get; set; }

    public string Specialization { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public decimal ConsultationFee { get; set; }

    public bool IsAvailable { get; set; }

    public User User { get; set; } = null!;

    public ICollection<AvailabilitySlot> AvailabilitySlots
        = new List<AvailabilitySlot>();
}