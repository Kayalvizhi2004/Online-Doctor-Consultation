import { Component, OnInit, inject } from '@angular/core';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  appointments: any[] = [];
  private service = inject(AppointmentService);
  private router = inject(Router);

  ngOnInit(): void {
    this.load();
    // Refresh every 30 seconds
    setInterval(() => this.load(), 30000);
  }

  load(status?: string): void {
    this.service.getAll(status).subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        this.appointments = list.sort((a: any, b: any) => {
          const dateA = new Date(a.date).getTime();
          const dateB = new Date(b.date).getTime();
          return dateB - dateA; // Most recent first
        });
      },
      error: (err) => {
        console.error('Failed to load appointments', err);
        this.appointments = [];
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
        next: () => {
          alert('Appointment cancelled successfully');
          this.load();
        },
        error: (err) => {
          console.error('Failed to cancel appointment', err);
          alert('Failed to cancel appointment');
        }
      });
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
      alert('Invalid appointment');
      return;
    }
    this.router.navigate(['/consultation', sessionId]);
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
