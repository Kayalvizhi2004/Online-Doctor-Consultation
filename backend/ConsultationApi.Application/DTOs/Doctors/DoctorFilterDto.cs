namespace ConsultationApi.Application.DTOs.Doctors;

public class DoctorFilterDto
{
    public string? Specialization { get; set; }
    public DateOnly? AvailableDate { get; set; }
}