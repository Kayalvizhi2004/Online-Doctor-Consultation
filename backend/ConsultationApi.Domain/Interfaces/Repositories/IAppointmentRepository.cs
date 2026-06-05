using ConsultationApi.Domain.Entities.Appointments;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task AddAsync(
        Appointment appointment);

    Task<Appointment?>
        GetByIdAsync(
            Guid id);

    Task<List<Appointment>>
        GetAppointmentsAsync(
            Guid userId,
            string role,
            string? status,
            int page,
            int pageSize);

    Task UpdateAsync(
        Appointment appointment);

    Task CreateSessionAsync(
        ConsultationSession session);

    Task<ConsultationSession?> GetSessionByAppointmentAsync(
        Guid appointmentId);

    Task<ConsultationSession?> GetSessionByIdAsync(
        Guid sessionId);

    Task UpdateSessionAsync(
        ConsultationSession session);

    Task SaveChangesAsync();
}