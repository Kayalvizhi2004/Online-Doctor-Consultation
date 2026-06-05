using AutoMapper;
using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.DTOs.Common;
using ConsultationApi.Application.Interfaces.Chat;
using ConsultationApi.Domain.Entities.Chat;
using ConsultationApi.Domain.Entities.Appointments;
using ConsultationApi.Domain.Interfaces.Repositories;

namespace ConsultationApi.Application.Services.Chat;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepo;
    private readonly IAppointmentRepository _appointments;
    private readonly IMapper _mapper;

    public ChatService(
        IChatRepository chatRepo,
        IAppointmentRepository appointments,
        IMapper mapper)
    {
        _chatRepo = chatRepo;
        _appointments = appointments;
        _mapper = mapper;
    }

    public async Task<bool> CanAccessSessionAsync(Guid sessionId, Guid userId)
    {
        var session = await _appointments.GetSessionByIdAsync(sessionId);

        if (session == null) return false;

        return session.Appointment.DoctorId == userId || session.Appointment.PatientId == userId;
    }

    public async Task<
        ApiResponse<PagedResponse<ChatMessageDto>>>
        GetMessagesAsync(
            Guid sessionId,
            int page,
            int pageSize)
    {
        var messages =
            await _chatRepo
                .GetMessagesAsync(
                    sessionId,
                    page,
                    pageSize);

        return new ApiResponse<
            PagedResponse<ChatMessageDto>>
        {
            Success = true,
            Data = new PagedResponse<
                ChatMessageDto>
            {
                Items = _mapper.Map<IEnumerable<ChatMessageDto>>(messages),
                Page = page,
                PageSize = pageSize,
                TotalCount = messages.Count
            }
        };
    }

    public async Task<ApiResponse<ChatMessageDto>>
        SendMessageAsync(
            Guid sessionId,
            Guid senderId,
            SendMessageDto dto)
    {
        var session =
            await _appointments
                .GetSessionByIdAsync(
                    sessionId);

        if (session == null)
        {
            return new ApiResponse<ChatMessageDto>
            {
                Success = false,
                Message = "Session not found"
            };
        }

        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            SenderId = senderId,
            Message = dto.Message,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _chatRepo.AddMessageAsync(message);

        await _chatRepo.SaveChangesAsync();

        return new ApiResponse<ChatMessageDto>
        {
            Success = true,
            Data = _mapper.Map<ChatMessageDto>(message)
        };
    }

    public async Task<ApiResponse<string>>
        MarkMessagesReadAsync(
            Guid sessionId,
            Guid userId)
    {
        await _chatRepo.MarkReadAsync(
            sessionId,
            userId);

        await _chatRepo.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Messages marked read"
        };
    }
}