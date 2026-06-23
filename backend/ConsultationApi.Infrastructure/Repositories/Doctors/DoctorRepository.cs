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
            DateOnly? date,
            string? search = null)
    {
        // Project only required columns to avoid referencing DB columns
        // that might be missing (e.g. availability_slots.created_at).
        var baseQuery = _context.DoctorProfiles
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(specialization))
        {
            baseQuery = baseQuery.Where(d => d.Specialization == specialization);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            // Free-text search over the doctor's name and specialization,
            // case-insensitive (Postgres ILIKE). Escape LIKE metacharacters
            // (\ % _) so a typed "%" or "_" is matched literally rather than
            // acting as a wildcard. Postgres ILIKE uses '\' as the escape char.
            var escaped = search.Trim()
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");
            var pattern = $"%{escaped}%";
            baseQuery = baseQuery.Where(d =>
                EF.Functions.ILike(d.User.FullName, pattern) ||
                EF.Functions.ILike(d.Specialization, pattern));
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

    public async Task<List<string>>
        GetSpecializationsAsync()
    {
        // Distinct, non-empty specializations actually present in the data,
        // so the filter dropdown reflects whatever doctors exist.
        return await _context.DoctorProfiles
            .Select(d => d.Specialization)
            .Where(s => s != null && s != "")
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();
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