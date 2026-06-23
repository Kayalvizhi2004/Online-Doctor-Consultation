using ConsultationApi.Domain.Entities.Doctors;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface IDoctorRepository
{
    Task<List<DoctorProfile>>
        GetDoctorsAsync(
            string? specialization,
            DateOnly? date,
            string? search = null);

    Task<List<string>>
        GetSpecializationsAsync();

    Task<DoctorProfile?>
        GetDoctorByIdAsync(
            Guid id);

    Task<DoctorProfile?>
        GetDoctorByUserIdAsync(
            Guid userId);

    Task UpdateAsync(
        DoctorProfile profile);

    Task SaveChangesAsync();
}
