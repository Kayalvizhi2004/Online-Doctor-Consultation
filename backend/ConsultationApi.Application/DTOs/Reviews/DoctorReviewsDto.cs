namespace ConsultationApi.Application.DTOs.Reviews;

public class DoctorReviewsDto
{
    public double AverageRating { get; set; }

    public int TotalReviews { get; set; }

    public IEnumerable<ReviewDto> Items { get; set; }
        = new List<ReviewDto>();
}