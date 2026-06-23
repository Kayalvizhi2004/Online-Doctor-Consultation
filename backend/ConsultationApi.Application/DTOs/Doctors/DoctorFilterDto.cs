namespace ConsultationApi.Application.DTOs.Doctors;

public class DoctorFilterDto
{
    public string? Search { get; set; }
    public string? Specialization { get; set; }
    public DateOnly? AvailableDate { get; set; }
}
