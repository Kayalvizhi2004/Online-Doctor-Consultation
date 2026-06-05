using ConsultationApi.Application.DTOs.Chat;
using ConsultationApi.Application.DTOs.Common;

namespace ConsultationApi.Application.Interfaces.Chat;

public interface IChatService
{
    Task<ApiResponse<PagedResponse<ChatMessageDto>>> GetMessagesAsync(
        Guid sessionId,
        int page,
        int pageSize);

    Task<bool> CanAccessSessionAsync(Guid sessionId, Guid userId);

    Task<ApiResponse<ChatMessageDto>> SendMessageAsync(
        Guid sessionId,
        Guid senderId,
        SendMessageDto dto);

    Task<ApiResponse<string>> MarkMessagesReadAsync(
        Guid sessionId,
        Guid userId);
}