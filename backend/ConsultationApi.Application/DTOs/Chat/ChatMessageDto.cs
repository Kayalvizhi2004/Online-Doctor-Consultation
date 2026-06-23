namespace ConsultationApi.Application.DTOs.Chat;

public class ChatMessageDto
{
    public Guid SenderId { get; set; }

    public string SenderName { get; set; }
        = string.Empty;

    public string Message { get; set; }
        = string.Empty;

    public string MessageType { get; set; }
        = "text";

    public string? AttachmentUrl { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
}
