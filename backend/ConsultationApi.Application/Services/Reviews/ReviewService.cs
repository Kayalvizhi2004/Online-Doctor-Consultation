using AutoMapper;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Reviews;
using ConsultationApi.Application.Interfaces.Reviews;
using ConsultationApi.Domain.Entities.Reviews;
using ConsultationApi.Domain.Enums;
using ConsultationApi.Domain.Interfaces.Repositories;

namespace ConsultationApi.Application.Services.Reviews;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviews;
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper _mapper;

    public ReviewService(
        IReviewRepository reviews,
        IAppointmentRepository appointments,
        IMapper mapper)
    {
        _reviews = reviews;
        _appointments = appointments;
        _mapper = mapper;
    }

    public async Task<ApiResponse<string>>
        AddReviewAsync(
            Guid patientId,
            Guid appointmentId,
            CreateReviewDto dto)
    {
        var appointment =
            await _appointments
                .GetByIdAsync(
                    appointmentId);

        if (appointment == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Appointment not found"
            };
        }

        if (appointment.PatientId != patientId)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Unauthorized"
            };
        }

        if (appointment.Status !=
            AppointmentStatus.Completed)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message =
                    "Review allowed only after completed consultation"
            };
        }

        var exists = await _reviews.ExistsAsync(appointmentId);

        if (exists)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message =
                    "Review already submitted"
            };
        }

        var review =
            new Review
            {
                Id = Guid.NewGuid(),
                AppointmentId =
                    appointmentId,
                PatientId =
                    patientId,
                Rating =
                    dto.Rating,
                Comment =
                    dto.Comment,
                CreatedAt =
                    DateTime.UtcNow
            };

        await _reviews.AddAsync(review);

        await _reviews.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message =
                "Review submitted"
        };
    }

    public async Task<
        ApiResponse<PagedResponse<ReviewDto>>>
        GetDoctorReviewsAsync(
            Guid doctorId,
            int page,
            int pageSize)
    {
        var reviews = await _reviews.GetDoctorReviewsAsync(doctorId, page, pageSize);

        return new ApiResponse<
            PagedResponse<ReviewDto>>
        {
            Success = true,
            Data = new PagedResponse<
                ReviewDto>
            {
                Items =
                    _mapper.Map<
                        IEnumerable<ReviewDto>>(
                            reviews)
            }
        };
    }
}