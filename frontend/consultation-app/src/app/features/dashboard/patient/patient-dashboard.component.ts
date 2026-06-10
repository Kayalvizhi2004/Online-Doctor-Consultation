import { Component, OnInit, inject, signal } from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './patient-dashboard.component.html',
  styleUrls: ['./patient-dashboard.component.scss']
})
export class PatientDashboardComponent implements OnInit {

  appointments = signal<any[]>([]);
  unreadCount = signal<number>(0);
  user: any = null;

  private appointmentService = inject(AppointmentService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);

  ngOnInit(): void {
    try { this.user = JSON.parse(localStorage.getItem('user') || 'null'); } catch { this.user = null; }
    this.loadAppointments();
    this.loadNotifications();
  }

  /** A consultation is joinable once the doctor has started it. */
  canJoin(a: any): boolean {
    return !!a?.sessionId && (a.status || '').toLowerCase() === 'inprogress';
  }

  join(sessionId: string): void {
    if (sessionId) this.router.navigate(['/chat', sessionId]);
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        this.appointments.set(list || []);
      },
      error: () => this.appointments.set([])
    });
  }

  loadNotifications(): void {
    this.notificationService.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.data ?? res?.Data ?? []);
        this.unreadCount.set((list || []).filter((n: any) => !(n.isRead ?? n.IsRead)).length);
      },
      error: () => this.unreadCount.set(0)
    });
  }
}

