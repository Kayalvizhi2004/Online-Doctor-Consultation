export interface ChatMessage {
  id: string;
  sessionId: string;
  senderId: string;
  senderName?: string;
  message: string;
  sentAt: string;
  isRead: boolean;
}