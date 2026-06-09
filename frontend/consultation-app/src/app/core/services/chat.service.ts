import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatService {

  private messagesSubject = new BehaviorSubject<any[]>([]);
  public messages$: Observable<any[]> = this.messagesSubject.asObservable();

  private typingUsersSubject = new BehaviorSubject<string[]>([]);
  public typingUsers$: Observable<string[]> = this.typingUsersSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  private currentSessionId: string | null = null;

  constructor(private api: ApiService) {}

  // HTTP-backed methods
  getMessages(sessionId: string) {
    return this.api.get(`/api/sessions/${sessionId}/messages`);
  }

  sendMessage(sessionId: string, data: any) {
    return this.api.post(`/api/sessions/${sessionId}/messages`, data);
  }

  getUnreadTotal() {
    // Backend exposes notifications; derive unread count from notifications
    return this.api.get<any[]>('/api/notifications').pipe(
      tap((res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.data ?? res?.Data ?? []);
        const unread = (list || []).filter((n: any) => n.isRead === false || n.is_read === false || n.read === false).length;
        this.unreadCountSubject.next(unread || 0);
      })
    );
  }

  markRead(sessionId: string) {
    return this.api.patch(`/api/sessions/${sessionId}/messages/read`, {}).pipe(
      tap(() => this.getUnreadTotal().subscribe())
    );
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

  sendRealtimeMessage(sessionId: string, text: string, senderId: string): Observable<any> {
    const payload = { text, senderId, sentAt: new Date().toISOString(), messageType: 'text' };
    // Optimistically push to local messages stream
    const current = this.messagesSubject.getValue();
    this.messagesSubject.next([...current, payload]);
    // Also persist via API
    return this.sendMessage(sessionId, payload);
  }

  sendImageMessage(sessionId: string, base64: string, senderId: string): Observable<any> {
    const payload = { content: base64, senderId, sentAt: new Date().toISOString(), messageType: 'image' };
    const current = this.messagesSubject.getValue();
    this.messagesSubject.next([...current, payload]);
    return this.sendMessage(sessionId, payload);
  }

  sendGifMessage(sessionId: string, gifUrl: string, senderId: string): Observable<any> {
    const payload = { content: gifUrl, senderId, sentAt: new Date().toISOString(), messageType: 'gif' };
    const current = this.messagesSubject.getValue();
    this.messagesSubject.next([...current, payload]);
    return this.sendMessage(sessionId, payload);
  }

  notifyTyping(sessionId: string, userId: string): void {
    // Simulate typing indicator - in real app would use SignalR
    const current = this.typingUsersSubject.getValue();
    if (!current.includes(userId)) {
      this.typingUsersSubject.next([...current, userId]);
      setTimeout(() => {
        const updated = this.typingUsersSubject.getValue().filter(u => u !== userId);
        this.typingUsersSubject.next(updated);
      }, 3000);
    }
  }

  endSession(sessionId: string): Observable<any> {
    return this.api.post(`/api/sessions/${sessionId}/end`, {});
  }
}