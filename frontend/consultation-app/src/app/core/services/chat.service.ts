import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatService {

  private messagesSubject = new BehaviorSubject<any[]>([]);
  public messages$: Observable<any[]> = this.messagesSubject.asObservable();

  private currentSessionId: string | null = null;

  constructor(private api: ApiService) {}

  // HTTP-backed methods
  getMessages(sessionId: string) {
    return this.api.get(`/api/sessions/${sessionId}/messages`);
  }

  sendMessage(sessionId: string, data: any) {
    return this.api.post(`/api/sessions/${sessionId}/messages`, data);
  }

  markRead(sessionId: string) {
    return this.api.patch(`/api/sessions/${sessionId}/messages/read`, {});
  }

  // Lightweight realtime stubs (no external SignalR dependency)
  initSignalR(_token?: string) {
    // No-op for now. In a real app, initialize SignalR connection here.
    return;
  }

  joinSession(sessionId: string) {
    this.currentSessionId = sessionId;
    // Load initial messages and push to subject
    this.getMessages(sessionId).subscribe((msgs: any) => this.messagesSubject.next(msgs || []));
  }

  leaveSession(_sessionId: string) {
    this.currentSessionId = null;
    this.messagesSubject.next([]);
  }

  sendRealtimeMessage(sessionId: string, text: string, senderId: string) {
    const payload = { text, senderId, sentAt: new Date().toISOString() };
    // Optimistically push to local messages stream
    const current = this.messagesSubject.getValue();
    this.messagesSubject.next([...current, payload]);
    // Also persist via API
    return this.sendMessage(sessionId, payload);
  }
}