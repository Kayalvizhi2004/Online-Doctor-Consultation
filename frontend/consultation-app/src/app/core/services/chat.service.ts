import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { SignalRService } from './signalr.service';

@Injectable({ providedIn: 'root' })
export class ChatService {

  private messagesSubject = new BehaviorSubject<any[]>([]);
  public messages$: Observable<any[]> = this.messagesSubject.asObservable();

  private typingUsersSubject = new BehaviorSubject<string[]>([]);
  public typingUsers$: Observable<string[]> = this.typingUsersSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  private currentSessionId: string | null = null;
  private hubConnected = false;

  constructor(private api: ApiService, private signalR: SignalRService) {}

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
    // Initialize SignalR connection and wire up message events.
    this.signalR.startConnection()
      .then(() => {
        this.hubConnected = true;
        this.signalR.onMessage((msg: any) => {
          const current = this.messagesSubject.getValue();
          this.messagesSubject.next([...current, msg]);
          // Refresh unread count when receiving message
          this.getUnreadTotal().subscribe();
        });
      })
      .catch((err: any) => {
        console.warn('SignalR connection failed, falling back to REST', err);
        this.hubConnected = false;
      });
  }

  joinSession(sessionId: string) {
    this.currentSessionId = sessionId;
    // Load initial messages and push to subject
    this.getMessages(sessionId).subscribe((msgs: any) => this.messagesSubject.next(msgs || []));
    if (this.hubConnected) {
      this.signalR.joinSession(sessionId).catch(() => {});
    }
  }

  leaveSession(_sessionId: string) {
    this.currentSessionId = null;
    this.messagesSubject.next([]);
  }

  sendRealtimeMessage(sessionId: string, text: string, senderId: string): Observable<any> {
    const payload = { message: text, messageType: 'text', sentAt: new Date().toISOString() };
    // If hub connected, use SignalR invoke and optimistically add
    if (this.hubConnected) {
      const current = this.messagesSubject.getValue();
      this.messagesSubject.next([...current, { ...payload, senderId }]);
      return new Observable((observer) => {
        this.signalR.sendMessage(sessionId, text)
          .then(() => { observer.next(null); observer.complete(); })
          .catch((err: any) => { observer.error(err); });
      });
    }

    // Fallback to REST
    const restPayload = { Message: text, MessageType: 'text' };
    const current = this.messagesSubject.getValue();
    this.messagesSubject.next([...current, { ...payload, senderId }]);
    return this.sendMessage(sessionId, restPayload);
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