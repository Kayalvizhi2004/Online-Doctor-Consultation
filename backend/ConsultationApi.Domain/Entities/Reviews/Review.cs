using ConsultationApi.Domain.Common;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Entities.Appointments;

namespace ConsultationApi.Domain.Entities.Reviews;

public class Review : BaseEntity
{
    public Guid AppointmentId { get; set; }

    public Guid PatientId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
    public User? Patient { get; set; }
    public Appointment? Appointment { get; set; }
}