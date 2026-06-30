import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ChatService } from '../../../../core/services/chat.service';
import { environment } from '../../../../../environments/environment';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit, OnDestroy {

  // Signal so the list renders as soon as data arrives (zoneless app).
  appointments = signal<any[]>([]);
  loading = signal<boolean>(true);

  // "View Summary" popup modal — shows the session's chat transcript.
  summaryAppt = signal<any | null>(null);
  transcript = signal<any[]>([]);
  transcriptLoading = signal<boolean>(false);
  private chat = inject(ChatService);

  role = signal<string>('');

  private service = inject(AppointmentService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private pollId: any;
  private userSub?: Subscription;
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.userSub = this.auth.user$.subscribe(u => this.role.set(u?.role || ''));
    this.load();
    // Refresh every 30 seconds
    this.pollId = setInterval(() => this.load(this.currentStatus), 30000);
  }

  isPatient(): boolean { return this.role() === 'Patient'; }
  isDoctor(): boolean { return this.role() === 'Doctor'; }

  ngOnDestroy(): void {
    if (this.pollId) clearInterval(this.pollId);
    this.userSub?.unsubscribe();
  }

  private currentStatus?: string;

  load(status?: string): void {
    this.currentStatus = status;
    this.loading.set(true);
    this.service.getAll(status).subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? []);
        const sorted = [...list].sort((a: any, b: any) => {
          const ta = new Date(a.createdAt || a.date).getTime();
          const tb = new Date(b.createdAt || b.date).getTime();
          return tb - ta; // Most recent first
        });
        this.appointments.set(sorted);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load appointments', err);
        this.appointments.set([]);
        this.loading.set(false);
      }
    });
  }

  filterStatus(event: Event | string) {
    let status = '';
    if (typeof event === 'string') status = event;
    else {
      const target = event.target as HTMLSelectElement | null;
      status = target ? target.value : '';
    }
    this.load(status || undefined);
  }

  canCancel(a: any): boolean {
    if (!a || (a.status?.toLowerCase() !== 'confirmed' && a.status?.toLowerCase() !== 'pending')) {
      return false;
    }
    const slot = new Date(a.date + ' ' + (a.startTime || '00:00'));
    return (slot.getTime() - Date.now()) > (60 * 60 * 1000); // At least 1 hour before
  }

  cancel(id: string) {
    if (confirm('Are you sure you want to cancel this appointment?')) {
      this.service.cancel(id).subscribe({
        next: () => this.load(this.currentStatus),
        error: (err) => {
          console.error('Failed to cancel appointment', err);
          this.toastr.error(err?.message || 'Failed to cancel appointment');
        }
      });
    }
  }

  /** Doctor action: confirm a pending appointment. */
  confirmAppointment(id: string) {
    this.service.confirm(id).subscribe({
      next: () => this.load(this.currentStatus),
      error: (err) => this.toastr.error(err?.message || 'Failed to confirm appointment')
    });
  }

  /** Doctor action: start the consultation session. */
  startSession(id: string) {
    this.service.startSession(id).subscribe({
      next: () => this.load(this.currentStatus),
      error: (err) => this.toastr.error(err?.message || 'Failed to start session')
    });
  }

  /** Doctor action: end the consultation session. */
  endSession(id: string) {
    this.service.endSession(id).subscribe({
      next: () => this.load(this.currentStatus),
      error: (err) => this.toastr.error(err?.message || 'Failed to end session')
    });
  }

  /** Whether this card shows any action button for the current role. */
  hasActions(a: any): boolean {
    if (this.isPatient()) return this.canCancel(a);
    if (this.isDoctor()) {
      const s = (a?.status || '').toLowerCase();
      return s === 'pending' || s === 'confirmed' || s === 'inprogress';
    }
    return false;
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

  /** Human-friendly status label (e.g. InProgress -> In Progress). */
  statusLabel(status: string): string {
    switch ((status || '').toLowerCase()) {
      case 'inprogress': return 'In Progress';
      default: return status || '';
    }
  }

  canJoin(a: any): boolean {
    if (!a || a.status?.toLowerCase() !== 'confirmed') return false;
    const slot = new Date(a.date + ' ' + (a.startTime || '00:00'));
    const now = Date.now();
    return (slot.getTime() - now) <= (15 * 60 * 1000) && (slot.getTime() - now) > (-60 * 60 * 1000);
  }

  join(sessionId: string) {
    if (!sessionId) {
      this.toastr.warning('Invalid appointment');
      return;
    }
    this.router.navigate(['/chat', sessionId]);
  }

  getStatusColor(status: string): string {
    switch ((status || '').toLowerCase()) {
      case 'pending': return '#f59e0b';
      case 'confirmed': return '#10b981';
      case 'inprogress': return '#8b5cf6';
      case 'completed': return '#3b82f6';
      case 'cancelled': return '#ef4444';
      default: return '#6b7280';
    }
  }

  getStatusBg(status: string): string {
    switch ((status || '').toLowerCase()) {
      case 'pending': return '#fef3c7';
      case 'confirmed': return '#d1fae5';
      case 'inprogress': return '#ede9fe';
      case 'completed': return '#dbeafe';
      case 'cancelled': return '#fee2e2';
      default: return '#f3f4f6';
    }
  }
}
