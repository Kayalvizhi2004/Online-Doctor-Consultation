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
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response =
            await _notificationService
                .GetNotificationsAsync(
                    pageNumber,
                    pageSize);

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
        var response =
            await _notificationService
                .MarkAsReadAsync(id);

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
        var response =
            await _notificationService
                .MarkAllAsReadAsync();

        return StatusCode(
            response.StatusCode,
            response);
    }
}