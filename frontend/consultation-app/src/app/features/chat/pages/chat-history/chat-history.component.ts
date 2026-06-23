import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { ChatService } from '../../../../core/services/chat.service';
import { AuthService } from '../../../../core/services/auth.service';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-chat-history',
  standalone: true,
  imports: [CommonModule, FormsModule, TimeAgoPipe],
  templateUrl: './chat-history.component.html',
  styleUrls: ['./chat-history.component.scss']
})
export class ChatHistoryComponent implements OnInit {

  // Signals so the view renders when data arrives (zoneless app).
  sessions = signal<any[]>([]);
  loading = signal<boolean>(true);
  searchQuery = '';

  selected = signal<any | null>(null);
  transcript = signal<any[]>([]);
  transcriptLoading = signal<boolean>(false);

  me = '';
  role = signal<string>('');

  private appt = inject(AppointmentService);
  private chat = inject(ChatService);
  private auth = inject(AuthService);

  ngOnInit(): void {
    this.auth.user$.subscribe(u => {
      this.me = u?.id || this.subFromToken();
      this.role.set(u?.role || '');
    });
    this.load();
  }

  isDoctor(): boolean { return this.role() === 'Doctor'; }

  load(): void {
    this.loading.set(true);
    this.appt.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        // Only appointments that actually have a consultation session (chat happened).
        const withSession = (list || [])
          .filter((a: any) => !!a.sessionId)
          .sort((a: any, b: any) => new Date(b.createdAt || b.date).getTime() - new Date(a.createdAt || a.date).getTime());
        this.sessions.set(withSession);
        this.loading.set(false);
      },
      error: () => { this.sessions.set([]); this.loading.set(false); }
    });
  }

  filtered(): any[] {
    const q = this.searchQuery.trim().toLowerCase();
    const list = this.sessions();
    if (!q) return list;
    return list.filter((s: any) =>
      (s.patientName || '').toLowerCase().includes(q) ||
      (s.doctorName || '').toLowerCase().includes(q)
    );
  }

  /** Other party (a doctor sees the patient; a patient sees the doctor). */
  party(s: any): string {
    return this.isDoctor() ? (s.patientName || 'Patient') : (s.doctorName || 'Doctor');
  }

  partyLabel(s: any): string {
    return this.isDoctor() ? 'Patient' : (s.specialization || 'Doctor');
  }

  view(s: any): void {
    this.selected.set(s);
    this.transcript.set([]);
    this.transcriptLoading.set(true);
    this.chat.getMessages(s.sessionId).subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        this.transcript.set((list || []).map((m: any) => this.normalizeMsg(m)));
        this.transcriptLoading.set(false);
      },
      error: () => { this.transcript.set([]); this.transcriptLoading.set(false); }
    });
  }

  close(): void {
    this.selected.set(null);
    this.transcript.set([]);
  }

  isMine(m: any): boolean {
    return (m?.senderId || '') === this.me;
  }

  private normalizeMsg(m: any) {
    const rawUrl = m.attachmentUrl ?? m.AttachmentUrl;
    return {
      senderId: m.senderId ?? m.SenderId,
      senderName: m.senderName ?? m.SenderName,
      message: m.message ?? m.Message ?? m.content ?? '',
      messageType: m.messageType ?? m.MessageType ?? 'text',
      attachmentUrl: rawUrl && rawUrl.startsWith('/') ? environment.apiUrl + rawUrl : (rawUrl || null),
      sentAt: m.sentAt ?? m.SentAt
    };
  }

  /** Build a .txt transcript client-side from the loaded messages (no backend endpoint needed). */
  download(s: any): void {
    const lines = this.transcript().map((m: any) =>
      `[${m.sentAt ? new Date(m.sentAt).toLocaleString() : ''}] ${m.senderName || m.senderId}: ${m.attachmentUrl ? `[${m.messageType}] ${m.attachmentUrl} ` : ''}${m.message}`);
    const header = `Consultation transcript\n${this.party(s)} · ${s.date || ''} ${s.startTime || ''}-${s.endTime || ''}\n\n`;
    const blob = new Blob([header + lines.join('\n')], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `transcript-${s.sessionId}.txt`;
    a.click();
    URL.revokeObjectURL(url);
  }

  private subFromToken(): string {
    try {
      return JSON.parse(atob((localStorage.getItem('token') || '').split('.')[1])).sub || '';
    } catch { return ''; }
  }
}

