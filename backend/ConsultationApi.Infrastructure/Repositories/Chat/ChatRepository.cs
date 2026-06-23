using ConsultationApi.Domain.Entities.Chat;
using ConsultationApi.Domain.Interfaces;
using ConsultationApi.Domain.Interfaces.Repositories;
using ConsultationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApi.Infrastructure.Repositories.Chat;

public class ChatRepository
    : IChatRepository
{
    private readonly AppDbContext _context;

    public ChatRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task AddMessageAsync(
        ChatMessage message)
    {
        await _context.ChatMessages
            .AddAsync(message);
    }

    public async Task<List<ChatMessage>>
        GetMessagesAsync(
            Guid sessionId,
            int page,
            int pageSize)
    {
        return await _context.ChatMessages
            .Include(x => x.Sender)
            .Where(
                x =>
                    x.SessionId ==
                    sessionId)
            .OrderBy(
                x => x.SentAt)
            .Skip(
                (page - 1)
                * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task MarkReadAsync(
        Guid sessionId,
        Guid userId)
    {
        var messages =
            await _context.ChatMessages
                .Where(
                    x =>
                        x.SessionId ==
                        sessionId
                        &&
                        x.SenderId !=
                        userId
                        &&
                        !x.IsRead)
                .ToListAsync();

        foreach (var msg in messages)
        {
            msg.IsRead = true;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

