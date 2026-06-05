namespace ConsultationApi.Application.DTOs.Appointments;

public class BookAppointmentDto
{
    public Guid DoctorId { get; set; }

    public Guid SlotId { get; set; }

    public string Notes { get; set; }
        = string.Empty;
}