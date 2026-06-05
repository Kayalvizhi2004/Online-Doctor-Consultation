using ConsultationApi.Domain.Entities.Doctors;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface ISlotRepository
{
    Task AddSlotAsync(
        AvailabilitySlot slot);

    Task<AvailabilitySlot?>
        GetByIdAsync(
            Guid id);

    Task DeleteAsync(
        AvailabilitySlot slot);

    Task SaveChangesAsync();

    Task UpdateAsync(
        AvailabilitySlot slot);
}