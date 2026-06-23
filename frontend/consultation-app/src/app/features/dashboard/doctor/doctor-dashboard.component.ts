import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
import { ChatService } from '../../../core/services/chat.service';
import { environment } from '../../../../environments/environment';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './doctor-dashboard.component.html',
  styleUrls: ['./doctor-dashboard.component.scss']
})
export class DoctorDashboardComponent implements OnInit, OnDestroy {

  // Signals so the view renders when data arrives (zoneless app).
  pending = signal<any[]>([]);
  confirmed = signal<any[]>([]);
  completed = signal<any[]>([]);
  stats = signal({ total: 0, pending: 0, confirmed: 0, completed: 0 });

  // "View Summary" popup modal — shows what happened in the session
  // (the chat transcript) for the selected completed appointment.
  summaryAppt = signal<any | null>(null);
  transcript = signal<any[]>([]);
  transcriptLoading = signal<boolean>(false);

  private appointmentService = inject(AppointmentService);
  private chat = inject(ChatService);
  private router = inject(Router);
  private refreshIntervalId: any;

  ngOnInit(): void {
    this.loadAppointments();
    this.refreshIntervalId = setInterval(() => this.loadAppointments(), 30000);
  }

  ngOnDestroy(): void {
    if (this.refreshIntervalId) clearInterval(this.refreshIntervalId);
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res)
          ? res
          : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        const by = (s: string) => (list || []).filter((a: any) => (a.status || '').toLowerCase() === s);

        const pending = by('pending');
        const confirmed = by('confirmed');
        const completed = by('completed');

        this.pending.set(pending);
        this.confirmed.set(confirmed);
        this.completed.set(completed);
        this.stats.set({
          total: (list || []).length,
          pending: pending.length,
          confirmed: confirmed.length,
          completed: completed.length
        });
      },
      error: () => {
        this.pending.set([]);
        this.confirmed.set([]);
        this.completed.set([]);
        this.stats.set({ total: 0, pending: 0, confirmed: 0, completed: 0 });
      }
    });
  }

  confirm(id: string): void {
    this.appointmentService.confirm(id).subscribe({
      next: () => this.loadAppointments(),
      error: (err: any) => alert(err?.message || 'Confirmation failed')
    });
  }

  startSession(id: string): void {
    this.appointmentService.startSession(id).subscribe({
      next: (sessionId: any) => {
        if (sessionId) this.router.navigate(['/chat', sessionId]);
        else this.loadAppointments();
      },
      error: (err: any) => alert(err?.message || 'Failed to start session')
    });
  }

  /** Open the modal and load the session's chat transcript. */
  viewSummary(appt: any): void {
    this.summaryAppt.set(appt);
    this.transcript.set([]);
    if (!appt?.sessionId) return;
    this.transcriptLoading.set(true);
    this.chat.getMessages(appt.sessionId).subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        this.transcript.set((list || []).map((m: any) => this.normalizeMsg(m)));
        this.transcriptLoading.set(false);
      },
      error: () => { this.transcript.set([]); this.transcriptLoading.set(false); }
    });
  }

  closeSummary(): void { this.summaryAppt.set(null); this.transcript.set([]); }

  private normalizeMsg(m: any) {
    const rawUrl = m.attachmentUrl ?? m.AttachmentUrl;
    return {
      senderName: m.senderName ?? m.SenderName ?? 'Unknown',
      message: m.message ?? m.Message ?? m.content ?? '',
      messageType: m.messageType ?? m.MessageType ?? 'text',
      attachmentUrl: rawUrl && rawUrl.startsWith('/') ? environment.apiUrl + rawUrl : (rawUrl || null),
      sentAt: m.sentAt ?? m.SentAt
    };
  }
}

