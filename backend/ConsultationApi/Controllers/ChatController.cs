using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.Interfaces.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConsultationApi.Controllers;

[ApiController]
[Authorize]
[Route("api/sessions/{sessionId:guid}/messages")]
public class ChatController : ControllerBase
{
    private static readonly Dictionary<string, string> AllowedImageTypes = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/gif"] = ".gif",
        ["image/webp"] = ".webp"
    };

    private const long MaxAttachmentBytes = 5 * 1024 * 1024; // 5 MB

    private readonly IChatService _chatService;
    private readonly IWebHostEnvironment _env;

    public ChatController(
        IChatService chatService,
        IWebHostEnvironment env)
    {
        _chatService = chatService;
        _env = env;
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
        var userId = GetUserId();

        // Only the session's own doctor and patient may read its messages.
        var allowed =
            await _chatService.CanAccessSessionAsync(
                sessionId,
                userId);

        if (!allowed)
        {
            return StatusCode(403,
                ApiResponse<object>.Failure(
                    "You are not a participant of this session", 403));
        }

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
    // POST attachments
    // Upload an image/GIF for this session; stored on local
    // disk under wwwroot/uploads and served as a static file.
    //-----------------------------------------------------

    [HttpPost("attachments")]
    [RequestSizeLimit(MaxAttachmentBytes)]
    public async Task<IActionResult> UploadAttachment(
        Guid sessionId,
        IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var allowed =
            await _chatService.CanAccessSessionAsync(
                sessionId,
                userId);

        if (!allowed)
        {
            return StatusCode(403,
                ApiResponse<object>.Failure(
                    "You are not a participant of this session", 403));
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(
                ApiResponse<object>.Failure("No file uploaded", 400));
        }

        if (file.Length > MaxAttachmentBytes)
        {
            return BadRequest(
                ApiResponse<object>.Failure("File exceeds the 5 MB limit", 400));
        }

        if (!AllowedImageTypes.TryGetValue(
                file.ContentType?.ToLowerInvariant() ?? "",
                out var extension))
        {
            return BadRequest(
                ApiResponse<object>.Failure(
                    "Only JPEG, PNG, GIF and WebP images are allowed", 400));
        }

        var uploadDir = Path.Combine(
            _env.ContentRootPath,
            "wwwroot",
            "uploads",
            "chat",
            sessionId.ToString());

        Directory.CreateDirectory(uploadDir);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(uploadDir, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/chat/{sessionId}/{fileName}";

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                Url = url,
                MessageType = extension == ".gif" ? "gif" : "image"
            },
            "Attachment uploaded"));
    }

    //-----------------------------------------------------
    // PATCH read
    // Mark all messages read
    //-----------------------------------------------------

    [HttpPatch("read")]
    public async Task<IActionResult> MarkRead(
        Guid sessionId)
    {
        var userId = GetUserId();

        var allowed =
            await _chatService.CanAccessSessionAsync(
                sessionId,
                userId);

        if (!allowed)
        {
            return StatusCode(403,
                ApiResponse<object>.Failure(
                    "You are not a participant of this session", 403));
        }

        var response =
            await _chatService.MarkMessagesReadAsync(
                sessionId,
                userId);

        return StatusCode(
            response.StatusCode,
            response);
    }

    //-----------------------------------------------------
    // Helper: current user's id from the JWT
    //-----------------------------------------------------

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst("sub")?.Value ??
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
}