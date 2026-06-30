import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-consultation-sessions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './consultation-sessions.component.html',
  styleUrls: ['./consultation-sessions.component.scss']
})
export class ConsultationSessionsComponent implements OnInit, OnDestroy {

  role = signal<string>('');
  active = signal<any[]>([]);    // InProgress (joinable now)
  upcoming = signal<any[]>([]);  // Confirmed (doctor can Start)
  loading = signal<boolean>(true);
  starting = signal<string | null>(null);

  private appt = inject(AppointmentService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private pollId: any;
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.auth.user$.subscribe(u => this.role.set(u?.role || ''));
    this.load();
    this.pollId = setInterval(() => this.load(), 20000); // keep both sides in sync
  }

  ngOnDestroy(): void {
    if (this.pollId) clearInterval(this.pollId);
  }

  isDoctor(): boolean { return this.role() === 'Doctor'; }

  load(): void {
    this.loading.set(true);
    this.appt.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? []);
        const by = (s: string) => list.filter((a: any) => (a.status || '').toLowerCase() === s);
        this.active.set(by('inprogress'));
        this.upcoming.set(by('confirmed'));
        this.loading.set(false);
      },
      error: () => { this.active.set([]); this.upcoming.set([]); this.loading.set(false); }
    });
  }

  /** Doctor: start a confirmed appointment, then jump into its chat room. */
  start(a: any): void {
    if (this.starting()) return;
    this.starting.set(a.id);
    this.appt.startSession(a.id).subscribe({
      next: (sessionId: any) => {
        this.starting.set(null);
        if (sessionId) this.router.navigate(['/chat', sessionId]);
        else this.load();
      },
      error: (err: any) => { this.starting.set(null); this.toastr.error(err?.message || 'Failed to start the session.'); }
    });
  }

  /** Open the live chat for an already-started session (doctor or patient). */
  open(a: any): void {
    if (a.sessionId) this.router.navigate(['/chat', a.sessionId]);
    else { this.toastr.info('Session is not active yet.'); this.load(); }
  }

  /** The "other party" name to show on the card. */
  partyName(a: any): string {
    return this.isDoctor() ? (a.patientName || 'Patient') : (a.doctorName || 'Doctor');
  }

  partyLabel(a: any): string {
    return this.isDoctor() ? 'Patient' : (a.specialization || 'Doctor');
  }
}
