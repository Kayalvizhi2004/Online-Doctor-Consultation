import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './doctor-dashboard.component.html',
  styleUrls: ['./doctor-dashboard.component.scss']
})
export class DoctorDashboardComponent implements OnInit {

  appointments: any[] = [];
  pending: any[] = [];
  confirmed: any[] = [];
  completed: any[] = [];
  activeSession: any = null;
  stats = { total: 0, pending: 0, confirmed: 0, completed: 0 };

  constructor(private appointmentService: AppointmentService, private router: Router) {}

  ngOnInit(): void {
    this.loadAppointments();
    // Refresh every 30 seconds
    setInterval(() => this.loadAppointments(), 30000);
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        this.appointments = list;
        this.pending = list.filter((a: any) => a.status?.toLowerCase() === 'pending');
        this.confirmed = list.filter((a: any) => a.status?.toLowerCase() === 'confirmed');
        this.completed = list.filter((a: any) => a.status?.toLowerCase() === 'completed');
        this.stats = {
          total: list.length,
          pending: this.pending.length,
          confirmed: this.confirmed.length,
          completed: this.completed.length
        };
      },
      error: (err) => {
        console.error('Failed to load doctor appointments', err);
        this.appointments = [];
        this.pending = [];
        this.confirmed = [];
        this.completed = [];
      }
    });
  }

  confirm(id: string): void {
    this.appointmentService.confirm(id).subscribe({
      next: () => {
        alert('Appointment confirmed successfully');
        this.loadAppointments();
      },
      error: (err) => console.error('Confirmation failed', err)
    });
  }

  startSession(id: string): void {
    this.appointmentService.startSession(id).subscribe({
      next: () => {
        this.router.navigate(['/consultation', id]);
      },
      error: (err) => console.error('Failed to start session', err)
    });
  }

  endSession(id: string): void {
    this.appointmentService.endSession(id).subscribe({
      next: () => {
        this.activeSession = null;
        this.loadAppointments();
      },
      error: (err) => console.error('Failed to end session', err)
    });
  }

  getStatusColor(status: string): string {
    switch ((status || '').toLowerCase()) {
      case 'pending': return '#f59e0b';
      case 'confirmed': return '#10b981';
      case 'completed': return '#3b82f6';
      case 'cancelled': return '#ef4444';
      default: return '#6b7280';
    }
  }

  getStatusBg(status: string): string {
    switch ((status || '').toLowerCase()) {
      case 'pending': return '#fef3c7';
      case 'confirmed': return '#d1fae5';
      case 'completed': return '#dbeafe';
      case 'cancelled': return '#fee2e2';
      default: return '#f3f4f6';
    }
  }
}