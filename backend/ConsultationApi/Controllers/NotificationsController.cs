using ConsultationApi.Application.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApi.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
 private readonly INotificationService _notificationService;

 public NotificationController(
 INotificationService notificationService)
 {
 _notificationService = notificationService;
 }

 //----------------------------------------------------
 // GET notifications
 // GET /api/notifications
 //----------------------------------------------------

 [HttpGet]
 public async Task<IActionResult> GetNotifications()
 {
 var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
 User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

 var response =
 await _notificationService
 .GetNotificationsAsync(userId);

 return StatusCode(
 response.StatusCode,
 response);
 }

 //----------------------------------------------------
 // PATCH read
 // PATCH /api/notifications/{id}/read
 //----------------------------------------------------

 [HttpPatch("{id:guid}/read")]
 public async Task<IActionResult> MarkAsRead(
 Guid id)
 {
 var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
 User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

 var response =
 await _notificationService
 .MarkReadAsync(id, userId);

 return StatusCode(
 response.StatusCode,
 response);
 }

 //----------------------------------------------------
 // PATCH read-all
 // PATCH /api/notifications/read-all
 //----------------------------------------------------

 [HttpPatch("read-all")]
 public async Task<IActionResult> MarkAllAsRead()
 {
 var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
 User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

 var response =
 await _notificationService
 .MarkAllReadAsync(userId);

 return StatusCode(
 response.StatusCode,
 response);
 }

 //----------------------------------------------------
 // DELETE notification
 // DELETE /api/notifications/{id}
 //----------------------------------------------------

 [HttpDelete("{id:guid}")]
 public async Task<IActionResult> Delete(
 Guid id)
 {
 var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
 User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

 var response =
 await _notificationService
 .DeleteAsync(id, userId);

 return StatusCode(
 response.StatusCode,
 response);
 }
}