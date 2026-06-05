using ConsultationApi.Domain.Entities.Notifications;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Notifications;

public class NotificationRepository
    : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Notification notification)
    {
        await _context.Notifications
            .AddAsync(notification);
    }

    public async Task<List<Notification>>
        GetByUserIdAsync(
            Guid userId)
    {
        return await _context.Notifications
            .Where(
                x =>
                    x.UserId ==
                    userId)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<
        Notification?> GetByIdAsync(
        Guid id)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    public async Task MarkAllReadAsync(
        Guid userId)
    {
        var notifications =
            await _context.Notifications
                .Where(
                    x =>
                        x.UserId ==
                        userId
                        &&
                        !x.IsRead)
                .ToListAsync();

        foreach (var n
            in notifications)
        {
            n.IsRead = true;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}