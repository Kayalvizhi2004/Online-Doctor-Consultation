namespace ConsultationApi.Application.DTOs.Reviews;

public class ReviewDto
{
    public Guid PatientId { get; set; }

    public string PatientName { get; set; }
        = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; }
        = string.Empty;

    public DateTime CreatedAt { get; set; }
}