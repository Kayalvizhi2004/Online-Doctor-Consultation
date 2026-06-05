namespace ConsultationApi.Application.DTOs.Reviews;

public class CreateReviewDto
{
    public int Rating { get; set; }

    public string Comment { get; set; }
        = string.Empty;
}