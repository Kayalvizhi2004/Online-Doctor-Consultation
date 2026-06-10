using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Notifications;

namespace ConsultationApi.Application.Interfaces.Notifications;

public interface INotificationService
{
 Task<ApiResponse<IEnumerable<NotificationDto>>> GetNotificationsAsync(
 Guid userId);

 Task<ApiResponse<string>> MarkReadAsync(
 Guid notificationId,
 Guid userId);

 Task<ApiResponse<string>> MarkAllReadAsync(
 Guid userId);

 Task<ApiResponse<string>> DeleteAsync(
 Guid notificationId,
 Guid userId);

 Task<ApiResponse<PagedResponse<NotificationDto>>>
 GetNotificationsAsync(
 int pageNumber,
 int pageSize);

 Task<ApiResponse<bool>>
 MarkAsReadAsync(Guid notificationId);

 Task<ApiResponse<bool>>
 MarkAllAsReadAsync();
}