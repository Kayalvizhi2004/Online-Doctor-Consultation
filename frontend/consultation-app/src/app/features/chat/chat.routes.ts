import { Routes } from '@angular/router';
import { ConsultationChatComponent } from './pages/consultation-chat/consultation-chat.component';

export const CHAT_ROUTES: Routes = [
  {
    path: ':sessionId',
    component: ConsultationChatComponent
  }
];