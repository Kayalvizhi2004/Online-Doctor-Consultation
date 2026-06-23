using ConsultationApi.Domain.Common;
using ConsultationApi.Domain.Entities.Users;
using ConsultationApi.Domain.Entities.Appointments;

namespace ConsultationApi.Domain.Entities.Chat;

public class ChatMessage : BaseEntity
{
    public Guid SessionId { get; set; }

    public Guid SenderId { get; set; }

    public string Message { get; set; } = string.Empty;

    /// "text" | "image" | "gif"
    public string MessageType { get; set; } = "text";

    /// Relative URL of an uploaded attachment, e.g. /uploads/chat/{sessionId}/{file}
    public string? AttachmentUrl { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
    public User? Sender { get; set; }

    public ConsultationSession? Session { get; set; }
}