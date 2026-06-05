using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.Interfaces.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApi.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions/{sessionId:guid}/messages")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(
        IChatService chatService)
    {
        _chatService = chatService;
    }

    //-----------------------------------------------------
    // GET messages
    // GET /api/sessions/{sessionId}/messages
    //-----------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> GetMessages(
        Guid sessionId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response =
            await _chatService.GetMessagesAsync(
                sessionId,
                pageNumber,
                pageSize);

        return StatusCode(
            response.StatusCode,
            response);
    }

    //-----------------------------------------------------
    // POST messages
    // REST fallback if SignalR unavailable
    //-----------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> SendMessage(
        Guid sessionId,
        [FromBody] SendMessageDto dto)
    {

        var senderId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _chatService.SendMessageAsync(
                sessionId,
                senderId,
                dto);

        return StatusCode(
            response.StatusCode,
            response);
    }

    //-----------------------------------------------------
    // PATCH read
    // Mark all messages read
    //-----------------------------------------------------

    [HttpPatch("read")]
    public async Task<IActionResult> MarkRead(
        Guid sessionId)
    {

        var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var response =
            await _chatService.MarkMessagesReadAsync(
                sessionId,
                userId);

        return StatusCode(
            response.StatusCode,
            response);
    }
}