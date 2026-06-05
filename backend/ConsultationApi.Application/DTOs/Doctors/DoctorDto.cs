namespace ConsultationApi.Application.DTOs.Doctors;

public class DoctorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal ConsultationFee { get; set; }
    public IEnumerable<AvailabilitySlotDto> Slots { get; set; } = Array.Empty<AvailabilitySlotDto>();
}