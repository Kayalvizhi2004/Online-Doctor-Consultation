import { AuthState } from './auth.state';
import { Notification } from '../models/notification.model';
import { ChatMessage } from '../models/chat.model';

export interface AppState {

  auth: AuthState;

  notifications: {
    list: Notification[];
    unreadCount: number;
  };

  chat: {
    activeSessionId: string | null;
    messages: ChatMessage[];
  };

  ui: {
    loading: boolean;
    error: string | null;
  };
}