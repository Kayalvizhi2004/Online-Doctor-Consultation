using ConsultationApi.Domain.Common;
using ConsultationApi.Domain.Enums;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Entities.Doctors;

namespace ConsultationApi.Domain.Entities.Appointments;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public Guid SlotId { get; set; }

    public AppointmentStatus Status { get; set; }

    public string? Notes { get; set; }

    public ConsultationSession? ConsultationSession { get; set; }
    public User? Patient { get; set; }

    public DoctorProfile? Doctor { get; set; }
    public ConsultationApi.Domain.Entities.Doctors.AvailabilitySlot? Slot { get; set; }
}