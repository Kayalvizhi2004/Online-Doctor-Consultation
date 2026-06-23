namespace ConsultationApi.Application.DTOs.Chat;

public class SendMessageDto
{
    public string Message { get; set; }
        = string.Empty;

    /// "text" | "image" | "gif"
    public string MessageType { get; set; }
        = "text";

    public string? AttachmentUrl { get; set; }
}