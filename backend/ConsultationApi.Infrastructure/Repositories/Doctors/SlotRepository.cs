using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Doctors;

public class SlotRepository
    : ISlotRepository
{
    private readonly AppDbContext _context;

    public SlotRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddSlotAsync(
        AvailabilitySlot slot)
    {
        await _context
            .AvailabilitySlots
            .AddAsync(slot);
    }

    public async Task<
        AvailabilitySlot?> GetByIdAsync(
        Guid id)
    {
        return await _context
            .AvailabilitySlots
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    public async Task DeleteAsync(
        AvailabilitySlot slot)
    {
        _context.AvailabilitySlots
            .Remove(slot);

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(
        AvailabilitySlot slot)
    {
        _context.AvailabilitySlots.Update(slot);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}