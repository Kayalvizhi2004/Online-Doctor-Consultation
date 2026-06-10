import { Routes } from '@angular/router';
import { ConsultationChatComponent } from './pages/consultation-chat/consultation-chat.component';
import { ChatHistoryComponent } from './pages/chat-history/chat-history.component';
import { activeSessionGuard } from '../../core/guards/active-session.guard';

export const CHAT_ROUTES: Routes = [
  {
    path: '',
    component: ChatHistoryComponent
  },
  {
    path: ':sessionId',
    component: ConsultationChatComponent,
    canDeactivate: [activeSessionGuard]
  }
];
