import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
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

  private appointmentService = inject(AppointmentService);
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
}