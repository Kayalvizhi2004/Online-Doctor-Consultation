namespace ConsultationApi.Application.DTOs.Appointments;

public class AppointmentFilterDto
{
    public string? Status { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}