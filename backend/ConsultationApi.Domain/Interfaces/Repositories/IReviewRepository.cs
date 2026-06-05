using ConsultationApi.Domain.Entities.Reviews;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface IReviewRepository
{
    Task AddAsync(
        Review review);

    Task<List<Review>>
        GetDoctorReviewsAsync(
            Guid doctorId,
            int page,
            int pageSize);

    Task SaveChangesAsync();

    Task<bool> ExistsAsync(Guid appointmentId);
}