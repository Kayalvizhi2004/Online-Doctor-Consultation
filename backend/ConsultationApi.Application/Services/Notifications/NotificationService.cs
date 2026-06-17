using AutoMapper;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.DTOs.Notifications;
using ConsultationApi.Application.Interfaces.Notifications;
using ConsultationApi.Domain.Interfaces.Repositories;

namespace ConsultationApi.Application.Services.Notifications;

public class NotificationService : INotificationService
{
 private readonly INotificationRepository _notifications;
 private readonly IMapper _mapper;

 public NotificationService(
 INotificationRepository notifications,
 IMapper mapper)
 {
 _notifications = notifications;
 _mapper = mapper;
 }

 public async Task<
 ApiResponse<IEnumerable<NotificationDto>>>
 GetNotificationsAsync(
 Guid userId)
 {
 var notifications =
 await _notifications
 .GetByUserIdAsync(userId);

 return new ApiResponse<IEnumerable<NotificationDto>>
 {
 Success = true,
 Data = _mapper.Map<IEnumerable<NotificationDto>>(notifications)
 };
 }

 public async Task<ApiResponse<string>>
 MarkReadAsync(
 Guid notificationId,
 Guid userId)
 {
 var notification =
 await _notifications
 .GetByIdAsync(
 notificationId);

 if (notification == null ||
 notification.UserId != userId)
 {
 return new ApiResponse<string>
 {
 Success = false,
 Message = "Notification not found"
 };
 }

 notification.IsRead = true;

 await _notifications.SaveChangesAsync();

 return new ApiResponse<string>
 {
 Success = true,
 Message = "Notification marked read"
 };
 }

 public async Task<ApiResponse<string>>
 MarkAllReadAsync(
 Guid userId)
 {
 await _notifications
 .MarkAllReadAsync(
 userId);

 await _notifications.SaveChangesAsync();

 return new ApiResponse<string>
 {
 Success = true,
 Message = "All notifications marked read"
 };
 }

 public async Task<ApiResponse<string>>
 DeleteAsync(
 Guid notificationId,
 Guid userId)
 {
 var notification =
 await _notifications.GetByIdAsync(notificationId);

 if (notification == null || notification.UserId != userId)
 {
 return new ApiResponse<string>
 {
 Success = false,
 Message = "Notification not found"
 };
 }

 await _notifications.DeleteAsync(notification);
 await _notifications.SaveChangesAsync();

 return new ApiResponse<string>
 {
 Success = true,
 Message = "Notification deleted"
 };
 }

 public async Task<ApiResponse<PagedResponse<NotificationDto>>>
 GetNotificationsAsync(int pageNumber, int pageSize)
 {
 var all = await _notifications.GetByUserIdAsync(Guid.Empty);

 var items = _mapper.Map<IEnumerable<NotificationDto>>(all)
 .Skip((pageNumber - 1) * pageSize)
 .Take(pageSize);

 return new ApiResponse<PagedResponse<NotificationDto>>
 {
 Success = true,
 Data = new PagedResponse<NotificationDto>
 {
 Items = items,
 Page = pageNumber,
 PageSize = pageSize,
 TotalCount = all.Count
 }
 };
 }

 public async Task<ApiResponse<bool>> MarkAsReadAsync(Guid notificationId)
 {
 var notification = await _notifications.GetByIdAsync(notificationId);

 if (notification == null)
 return new ApiResponse<bool> { Success = false };

 notification.IsRead = true;
 await _notifications.SaveChangesAsync();

 return new ApiResponse<bool> { Success = true, Data = true };
 }

 public async Task<ApiResponse<bool>> MarkAllAsReadAsync()
 {
 await _notifications.MarkAllReadAsync(Guid.Empty);
 await _notifications.SaveChangesAsync();
 return new ApiResponse<bool> { Success = true, Data = true };
 }
}
