import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './patient-dashboard.component.html',
  styleUrls: ['./patient-dashboard.component.scss']
})
export class PatientDashboardComponent implements OnInit {

  appointments: any[] = [];
  notifications: any[] = [];
  unreadCount = 0;
  user: any = null;

  constructor(
    private appointmentService: AppointmentService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    try { this.user = JSON.parse(localStorage.getItem('user') || 'null'); } catch { this.user = null; }
    this.loadAppointments();
    this.loadNotifications();
  }

  join(sessionId: string) {
    if (!sessionId) return;
    window.location.href = `/consultation/${sessionId}`;
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe((res: any) => {
      this.appointments = res;
    });
  }

  loadNotifications(): void {
    this.notificationService.getAll().subscribe((res: any) => {
      this.notifications = res;
      this.unreadCount = res.filter((n: any) => !n.isRead).length;
    });
  }
}