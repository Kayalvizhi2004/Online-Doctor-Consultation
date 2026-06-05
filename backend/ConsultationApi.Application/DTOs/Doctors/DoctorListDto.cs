namespace ConsultationApi.Application.DTOs.Doctors;

public class DoctorListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal ConsultationFee { get; set; }
}
