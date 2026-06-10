using ConsultationApi.Domain.Entities.Appointments;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Appointments;

public class AppointmentRepository
    : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Appointment appointment)
    {
        await _context
            .Appointments
            .AddAsync(
                appointment);
    }

    public async Task<
        Appointment?> GetByIdAsync(
        Guid id)
    {
        return await _context
            .Appointments
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
                .ThenInclude(d => d!.User)
            .Include(x => x.Slot)
            .Include(x =>
                x.ConsultationSession)
            .FirstOrDefaultAsync(
                x => x.Id == id);
        // GetByIdAsync already includes ConsultationSession.
    }

    public async Task<
        List<Appointment>>
        GetAppointmentsAsync(
            Guid userId,
            string role,
            string? status,
            int page,
            int pageSize)
    {
        var query =
            _context.Appointments
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d!.User)
                .Include(x => x.Slot)
                .Include(x => x.ConsultationSession)
                .AsQueryable();

        if (role == "Patient")
        {
            query = query.Where(
                x =>
                    x.PatientId ==
                    userId);
        }

        if (role == "Doctor")
        {
            // userId is the logged-in user's id; appointments reference the
            // doctor PROFILE id, so match through the profile's UserId.
            query = query.Where(
                x =>
                    x.Doctor!.UserId ==
                    userId);
        }

        if (!string.IsNullOrWhiteSpace(
            status))
        {
            query = query.Where(
                x =>
                    x.Status.ToString()
                    == status);
        }

        return await query
            .OrderByDescending(
                x => x.CreatedAt)
            .Skip(
                (page - 1)
                * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetAppointmentsCountAsync(
        Guid userId,
        string role,
        string? status)
    {
        var query =
            _context.Appointments
                .AsQueryable();

        if (role == "Patient")
        {
            query = query.Where(x => x.PatientId == userId);
        }

        if (role == "Doctor")
        {
            query = query.Where(x => x.Doctor!.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status.ToString() == status);
        }

        return await query.CountAsync();
    }

    public async Task UpdateAsync(
        Appointment appointment)
    {
        _context.Appointments
            .Update(
                appointment);

        await Task.CompletedTask;
    }

    public async Task CreateSessionAsync(
        ConsultationSession session)
    {
        await _context.ConsultationSessions.AddAsync(session);
    }

    public async Task<ConsultationSession?> GetSessionByAppointmentAsync(
        Guid appointmentId)
    {
        return await _context.ConsultationSessions
            .Include(s => s.Appointment)
                .ThenInclude(a => a!.Doctor)
                    .ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);
    }

    public async Task<ConsultationSession?> GetSessionByIdAsync(
        Guid sessionId)
    {
        return await _context.ConsultationSessions
            .Include(s => s.Appointment)
                .ThenInclude(a => a!.Doctor)
                    .ThenInclude(d => d!.User)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task UpdateSessionAsync(
        ConsultationSession session)
    {
        _context.ConsultationSessions.Update(session);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
