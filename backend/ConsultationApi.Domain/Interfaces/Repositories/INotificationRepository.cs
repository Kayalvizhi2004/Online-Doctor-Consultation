using ConsultationApi.Domain.Entities.Notifications;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface INotificationRepository
{
    Task AddAsync(
        Notification notification);

    Task<List<Notification>>
        GetByUserIdAsync(
            Guid userId);

    Task<Notification?>
        GetByIdAsync(
            Guid id);

    Task MarkAllReadAsync(
        Guid userId);

    Task SaveChangesAsync();
}