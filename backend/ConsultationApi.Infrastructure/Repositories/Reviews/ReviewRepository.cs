using ConsultationApi.Domain.Entities.Reviews;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Reviews;

public class ReviewRepository
    : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Review review)
    {
        await _context.Reviews
            .AddAsync(review);
    }

    public async Task<List<Review>>
        GetDoctorReviewsAsync(
            Guid doctorId,
            int page,
            int pageSize)
    {
        return await _context.Reviews
            .Include(
                x =>
                    x.Appointment)
            .Where(
                x =>
                    x.Appointment != null &&
                    x.Appointment.DoctorId ==
                    doctorId)
            .OrderByDescending(
                x => x.CreatedAt)
            .Skip(
                (page - 1)
                * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid appointmentId)
    {
        return await _context.Reviews.AnyAsync(r => r.AppointmentId == appointmentId);
    }
}