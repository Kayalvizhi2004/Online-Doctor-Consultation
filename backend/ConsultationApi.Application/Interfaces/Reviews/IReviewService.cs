using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Reviews;

namespace ConsultationApi.Application.Interfaces.Reviews;

public interface IReviewService
{
    Task<ApiResponse<string>> AddReviewAsync(
        Guid patientId,
        Guid appointmentId,
        CreateReviewDto dto);

    Task<ApiResponse<DoctorReviewsDto>> GetDoctorReviewsAsync(
        Guid doctorId,
        int page,
        int pageSize);

    Task<ApiResponse<DoctorReviewsDto>> GetMyReviewsAsync(
        Guid doctorUserId,
        int page,
        int pageSize);
}