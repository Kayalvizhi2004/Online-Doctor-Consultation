using ConsultationApi.Domain.Entities.Chat;

namespace ConsultationApi.Domain.Interfaces.Repositories;

public interface IChatRepository
{
    Task AddMessageAsync(
        ChatMessage message);

    Task<List<ChatMessage>>
        GetMessagesAsync(
            Guid sessionId,
            int page,
            int pageSize);

    Task MarkReadAsync(
        Guid sessionId,
        Guid userId);

    Task SaveChangesAsync();
}