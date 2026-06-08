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
        // Project only required columns to avoid referencing DB columns
        // that might be missing (e.g. availability_slots.created_at).
        var baseQuery = _context.DoctorProfiles
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(specialization))
        {
            baseQuery = baseQuery.Where(d => d.Specialization == specialization);
        }

        if (date.HasValue)
        {
            baseQuery = baseQuery.Where(d => d.AvailabilitySlots.Any(s => s.Date == date && !s.IsBooked));
        }

        var projected = await baseQuery
            .Select(d => new DoctorProfile
            {
                Id = d.Id,
                Bio = d.Bio,
                ConsultationFee = d.ConsultationFee,
                CreatedAt = d.CreatedAt,
                IsAvailable = d.IsAvailable,
                Specialization = d.Specialization,
                UpdatedAt = d.UpdatedAt,
                UserId = d.UserId,
                User = new ConsultationApi.Domain.Entities.Users.User
                {
                    Id = d.User.Id,
                    Email = d.User.Email,
                    FullName = d.User.FullName,
                    Phone = d.User.Phone,
                    Role = d.User.Role,
                    CreatedAt = d.User.CreatedAt,
                    UpdatedAt = d.User.UpdatedAt
                },
                AvailabilitySlots = d.AvailabilitySlots
                    .Select(s => new AvailabilitySlot
                    {
                        Id = s.Id,
                        Date = s.Date,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsBooked = s.IsBooked,
                        DoctorId = s.DoctorId,
                        UpdatedAt = s.UpdatedAt
                    })
                    .ToList()
            })
            .ToListAsync();

        return projected;
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