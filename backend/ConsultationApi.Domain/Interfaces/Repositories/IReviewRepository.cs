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

    Task<(double Average, int Count)>
        GetDoctorRatingSummaryAsync(
            Guid doctorId);

    Task SaveChangesAsync();

    Task<bool> ExistsAsync(Guid appointmentId);

    Task<List<Guid>> GetReviewedAppointmentIdsAsync(Guid patientId);
}


