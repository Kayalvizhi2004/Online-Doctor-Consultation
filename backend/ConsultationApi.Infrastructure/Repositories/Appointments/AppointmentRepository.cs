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
            .Include(x => x.Slot)
            .Include(x =>
                x.ConsultationSession)
            .FirstOrDefaultAsync(
                x => x.Id == id);
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
                .Include(x => x.Slot)
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
            query = query.Where(
                x =>
                    x.DoctorId ==
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
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId);
    }

    public async Task<ConsultationSession?> GetSessionByIdAsync(
        Guid sessionId)
    {
        return await _context.ConsultationSessions
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