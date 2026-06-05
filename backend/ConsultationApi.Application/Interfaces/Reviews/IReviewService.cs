using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Reviews;

namespace ConsultationApi.Application.Interfaces.Reviews;

public interface IReviewService
{
    Task<ApiResponse<string>> AddReviewAsync(
        Guid patientId,
        Guid appointmentId,
        CreateReviewDto dto);

    Task<ApiResponse<PagedResponse<ReviewDto>>> GetDoctorReviewsAsync(
        Guid doctorId,
        int page,
        int pageSize);
}