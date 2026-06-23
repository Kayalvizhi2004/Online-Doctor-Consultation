import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject, Observable, Subject, from, map, tap } from 'rxjs';
import { SignalRService } from './signalr.service';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ChatService {

  private messagesSubject = new BehaviorSubject<any[]>([]);
  public messages$: Observable<any[]> = this.messagesSubject.asObservable();

  private sessionEndedSubject = new Subject<any>();
  public sessionEnded$: Observable<any> = this.sessionEndedSubject.asObservable();

  private connectedSubject = new BehaviorSubject<boolean>(false);
  public connected$ = this.connectedSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  private currentSessionId: string | null = null;

  constructor(private api: ApiService, private signalR: SignalRService) {}

  // ---- REST ----
  getMessages(sessionId: string, pageSize: number = 200) {
    return this.api.get(`/api/sessions/${sessionId}/messages`, { pageNumber: 1, pageSize });
  }

  markRead(sessionId: string) {
    return this.api.patch(`/api/sessions/${sessionId}/messages/read`, {});
  }

  /** Upload an image/GIF; the server stores it on local disk and returns its URL + type. */
  uploadAttachment(sessionId: string, file: File): Observable<{ url: string; messageType: string }> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.api.post<any>(`/api/sessions/${sessionId}/messages/attachments`, form).pipe(
      map((d: any) => ({
        url: d?.url ?? d?.Url,
        messageType: d?.messageType ?? d?.MessageType ?? 'image'
      }))
    );
  }

  /** Used by the navbar badge — derives an unread count from notifications. */
  getUnreadTotal() {
    return this.api.get<any[]>('/api/notifications').pipe(
      tap((res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.data ?? res?.Data ?? []);
        const unread = (list || []).filter((n: any) => n.isRead === false || n.IsRead === false).length;
        this.unreadCountSubject.next(unread || 0);
      })
    );
  }

  // ---- Normalize message shapes ----
  // REST history is PascalCase (SenderId/Message/SentAt); live hub messages are
  // camelCase (senderId/message/sentAt) and have no messageType.
  private normalize(m: any): any {
    if (!m) return m;
    return {
      id: m.id ?? m.Id,
      senderId: m.senderId ?? m.SenderId,
      senderName: m.senderName ?? m.SenderName,
      message: m.message ?? m.Message ?? m.content ?? '',
      content: m.content ?? m.Content,
      messageType: m.messageType ?? m.MessageType ?? 'text',
      attachmentUrl: this.absolutize(m.attachmentUrl ?? m.AttachmentUrl),
      sentAt: m.sentAt ?? m.SentAt ?? new Date().toISOString(),
      isRead: m.isRead ?? m.IsRead ?? false
    };
  }

  /** Attachment URLs are stored relative (/uploads/...); point them at the API host. */
  private absolutize(url: string | null | undefined): string | null {
    if (!url) return null;
    return url.startsWith('/') ? environment.apiUrl + url : url;
  }

  // ---- Realtime ----
  async init(sessionId: string): Promise<void> {
    this.currentSessionId = sessionId;

    // Load history first.
    this.getMessages(sessionId).subscribe((res: any) => {
      const items = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
      this.messagesSubject.next((items || []).map((m: any) => this.normalize(m)));
    });

    try {
      await this.signalR.startConnection();
      this.connectedSubject.next(true);

      this.signalR.onMessage((msg: any) => {
        const current = this.messagesSubject.getValue();
        this.messagesSubject.next([...current, this.normalize(msg)]);
      });

      this.signalR.onSessionEnded((payload: any) => {
        this.sessionEndedSubject.next(payload);
      });

      await this.signalR.joinSession(sessionId);
    } catch (err) {
      console.error('[chat] SignalR connection failed', err);
      this.connectedSubject.next(false);
      throw err;
    }
  }

  /** Send a live message. The server echoes it back via ReceiveMessage (incl. to us),
   *  so we do NOT optimistically append here to avoid duplicates. */
  sendMessage(sessionId: string, text: string, messageType: string = 'text', attachmentUrl: string | null = null): Observable<void> {
    // Ensure we pass `undefined` rather than `null` for optional hub args.
    return from(Promise.resolve(this.signalR.sendMessage(sessionId, text, messageType, attachmentUrl ?? undefined)).then(() => undefined));
  }

  /** Doctor-only on the server. */
  endSession(sessionId: string): Observable<void> {
    return from(Promise.resolve(this.signalR.endSession(sessionId)).then(() => undefined));
  }

  leave(sessionId: string): void {
    this.signalR.leaveSession(sessionId)?.catch(() => {});
    this.signalR.stop()?.catch(() => {});
    this.messagesSubject.next([]);
    this.connectedSubject.next(false);
    this.currentSessionId = null;
  }
}
