using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.Interfaces.Appointments;
using ConsultationApi.Application.Interfaces.Chat;
using ConsultationApi.Domain.Interfaces.Repositories;
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
    private readonly IAppointmentService _appointmentService;
    private readonly IAppointmentRepository _appointments;

    public ConsultationHub(
        IChatService chatService,
        IPresenceService presenceService,
        IAppointmentService appointmentService,
        IAppointmentRepository appointments)
    {
        _chatService = chatService;
        _presenceService = presenceService;
        _appointmentService = appointmentService;
        _appointments = appointments;
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
        string text,
        string? messageType,
        string? attachmentUrl)
    {
        var userId = GetUserId();

        //------------------------------------------------
        // Persist message
        //------------------------------------------------

        var dto = new SendMessageDto
        {
            Message = text ?? string.Empty,
            MessageType = messageType ?? "text",
            AttachmentUrl = attachmentUrl
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
                message = text,
                messageType = result.Data?.MessageType ?? "text",
                attachmentUrl = result.Data?.AttachmentUrl,
                sentAt = DateTime.UtcNow
            });
    }

    //----------------------------------------------------
    // EndSession (doctor only) — ends the consultation and
    // notifies everyone in the room so the patient is released.
    //----------------------------------------------------

    public async Task EndSession(
        Guid sessionId)
    {
        var userId = GetUserId();

        // Only a participant who is the Doctor may end the session.
        var allowed = await _chatService
            .CanAccessSessionAsync(sessionId, userId);

        var isDoctor = Context.User?.IsInRole("Doctor") ?? false;

        if (!allowed || !isDoctor)
        {
            throw new HubException(
                "Only the consulting doctor can end this session.");
        }

        var session = await _appointments.GetSessionByIdAsync(sessionId);
        if (session == null)
        {
            throw new HubException("Session not found.");
        }

        var result = await _appointmentService
            .EndSessionAsync(userId, session.AppointmentId);

        if (!result.Success)
        {
            throw new HubException(result.Message);
        }

        await Clients
            .Group(sessionId.ToString())
            .SessionEnded(new
            {
                sessionId,
                endedAt = DateTime.UtcNow,
                endedBy = "Doctor"
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