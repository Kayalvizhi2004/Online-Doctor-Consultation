import { Routes } from '@angular/router';
import { ConsultationChatComponent } from './pages/consultation-chat/consultation-chat.component';
import { ChatHistoryComponent } from './pages/chat-history/chat-history.component';

export const CHAT_ROUTES: Routes = [
  {
    path: '',
    component: ChatHistoryComponent
  },
  {
    path: ':sessionId',
    component: ConsultationChatComponent
  }
];