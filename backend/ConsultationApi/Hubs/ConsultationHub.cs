using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.Interfaces.Chat;
using ConsultationApi.Infrastructure.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ConsultationApi.Hubs;

[Authorize]
public class ConsultationHub
    : Hub<IConsultationClient>
{
    private readonly IChatService _chatService;
    private readonly IPresenceService _presenceService;

    public ConsultationHub(
        IChatService chatService,
        IPresenceService presenceService)
    {
        _chatService = chatService;
        _presenceService = presenceService;
    }

    //----------------------------------------------------
    // JoinSession
    //----------------------------------------------------

    public async Task JoinSession(
        Guid sessionId)
    {
        var userId = GetUserId();

        //------------------------------------------------
        // Validate participant
        //------------------------------------------------

        var allowed =
            await _chatService
                .CanAccessSessionAsync(
                    sessionId,
                    userId);

        if (!allowed)
        {
            throw new HubException(
                "Unauthorized session access.");
        }

        //------------------------------------------------
        // Add SignalR group
        //------------------------------------------------

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            sessionId.ToString());

        //------------------------------------------------
        // Store Redis presence
        //------------------------------------------------

        await _presenceService.AddConnectionAsync(
            sessionId,
            userId,
            Context.ConnectionId);

        //------------------------------------------------
        // Notify group
        //------------------------------------------------

        await Clients
            .Group(sessionId.ToString())
            .UserJoined(new
            {
                userId,
                name = Context.User?.Identity?.Name
            });
    }

    //----------------------------------------------------
    // LeaveSession
    //----------------------------------------------------

    public async Task LeaveSession(
        Guid sessionId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            sessionId.ToString());

        await _presenceService
            .RemoveConnectionAsync(
                sessionId,
                Context.ConnectionId);
    }

    //----------------------------------------------------
    // SendMessage
    //----------------------------------------------------

    public async Task SendMessage(
        Guid sessionId,
        string text)
    {
        var userId = GetUserId();

        //------------------------------------------------
        // Persist message
        //------------------------------------------------

        var dto = new SendMessageDto
        {
            Message = text
        };

        var result =
            await _chatService
                .SendMessageAsync(
                    sessionId,
                    userId,
                    dto);

        if (!result.Success)
        {
            throw new HubException(
                result.Message);
        }

        //------------------------------------------------
        // Broadcast
        //------------------------------------------------

        await Clients
            .Group(sessionId.ToString())
            .ReceiveMessage(new
            {
                senderId = userId,
                senderName = Context.User?.Identity?.Name,
                text,
                sentAt = DateTime.UtcNow
            });
    }

    //----------------------------------------------------
    // MarkRead
    //----------------------------------------------------

    public async Task MarkRead(
        Guid sessionId)
    {
        var userId = GetUserId();

        await _chatService
            .MarkMessagesReadAsync(
                sessionId,
                userId);
    }

    //----------------------------------------------------
    // Disconnect cleanup
    //----------------------------------------------------

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        await _presenceService
            .RemoveConnectionAsync(
                Context.ConnectionId);

        await base.OnDisconnectedAsync(
            exception);
    }

    //----------------------------------------------------
    // Helper
    //----------------------------------------------------

    private Guid GetUserId()
    {
        var value =
            Context.User?
                .FindFirst(
                    ClaimTypes.NameIdentifier)
                ?.Value;

        return Guid.Parse(value!);
    }
}