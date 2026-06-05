namespace ConsultationApi.Application.DTOs.Appointments;

public class AppointmentDto
{
    public Guid Id { get; set; }

    public string DoctorName { get; set; }
        = string.Empty;

    public string PatientName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public string Notes { get; set; }
        = string.Empty;

    public DateTime CreatedAt { get; set; }
}