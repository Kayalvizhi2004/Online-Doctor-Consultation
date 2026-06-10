
namespace ConsultationApi.Application.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }

    public Guid DoctorId { get; set; }

    public string DoctorName { get; set; }
        = string.Empty;

    public string Specialization { get; set; }
        = string.Empty;

    public string PatientName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public string Notes { get; set; }
        = string.Empty;

    public decimal ConsultationFee { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    // Set once a consultation session has been started for this appointment.
    public Guid? SessionId { get; set; }

    public DateTime CreatedAt { get; set; }
}