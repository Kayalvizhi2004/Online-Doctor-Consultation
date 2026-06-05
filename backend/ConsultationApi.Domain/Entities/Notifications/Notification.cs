using ConsultationApi.Domain.Common;
using ConsultationApi.Domain.Enums;

namespace ConsultationApi.Domain.Entities.Notifications;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }
}