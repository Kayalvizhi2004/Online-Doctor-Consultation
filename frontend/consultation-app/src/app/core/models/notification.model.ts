export interface Notification {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: 'Appointment' | 'Chat' | 'System' | 'Review';

  isRead: boolean;
  createdAt: string;
}