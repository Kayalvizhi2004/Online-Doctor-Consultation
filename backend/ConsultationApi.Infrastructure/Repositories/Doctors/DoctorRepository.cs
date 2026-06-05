using ConsultationApi.Domain.Entities.Doctors;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Doctors;

public class DoctorRepository
    : IDoctorRepository
{
    private readonly AppDbContext _context;

    public DoctorRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DoctorProfile>>
        GetDoctorsAsync(
            string? specialization,
            DateOnly? date)
    {
        var query =
            _context.DoctorProfiles
                .Include(x => x.User)
                .Include(x =>
                    x.AvailabilitySlots)
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(
            specialization))
        {
            query = query.Where(
                x =>
                    x.Specialization ==
                    specialization);
        }

        if (date.HasValue)
        {
            query = query.Where(
                x =>
                    x.AvailabilitySlots
                        .Any(
                            s =>
                                s.Date ==
                                date &&
                                !s.IsBooked));
        }

        return await query.ToListAsync();
    }

    public async Task<DoctorProfile?>
        GetDoctorByIdAsync(
            Guid id)
    {
        return await _context
            .DoctorProfiles
            .Include(x => x.User)
            .Include(x =>
                x.AvailabilitySlots)
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    public async Task<DoctorProfile?>
        GetDoctorByUserIdAsync(
            Guid userId)
    {
        return await _context
            .DoctorProfiles
            .Include(x => x.User)
            .Include(x => x.AvailabilitySlots)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task UpdateAsync(
        DoctorProfile profile)
    {
        _context.DoctorProfiles
            .Update(profile);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}