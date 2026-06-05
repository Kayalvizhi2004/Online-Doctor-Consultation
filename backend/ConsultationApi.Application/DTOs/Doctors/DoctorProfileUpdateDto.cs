namespace ConsultationApi.Application.DTOs.Doctors;

public class DoctorProfileUpdateDto
{
    public string Bio { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public bool IsAvailable { get; set; }
}