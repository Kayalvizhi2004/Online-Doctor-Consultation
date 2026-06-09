import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';

@Component({
  selector: 'app-consultation-sessions',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './consultation-sessions.component.html',
  styleUrls: ['./consultation-sessions.component.scss']
})
export class ConsultationSessionsComponent implements OnInit {

  sessions: any[] = [];
  upcomingSessions: any[] = [];
  activeSessions: any[] = [];
  isLoading = false;

  constructor(
    private appointmentService: AppointmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSessions();
    setInterval(() => this.loadSessions(), 60000); // Refresh every minute
  }

  loadSessions(): void {
    this.isLoading = true;
    this.appointmentService.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        this.sessions = list;
        
        const now = new Date();
        this.upcomingSessions = list.filter((s: any) => {
          const sessionTime = new Date(s.date + ' ' + (s.startTime || '00:00'));
          return sessionTime > now && (s.status?.toLowerCase() === 'confirmed');
        }).sort((a: any, b: any) => {
          const timeA = new Date(a.date + ' ' + (a.startTime || '00:00')).getTime();
          const timeB = new Date(b.date + ' ' + (b.startTime || '00:00')).getTime();
          return timeA - timeB;
        });

        this.activeSessions = list.filter((s: any) => s.status?.toLowerCase() === 'in_progress' || s.status?.toLowerCase() === 'inprogress');
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load sessions', err);
        this.sessions = [];
        this.upcomingSessions = [];
        this.activeSessions = [];
        this.isLoading = false;
      }
    });
  }

  startConsultation(sessionId: string): void {
    this.appointmentService.startSession(sessionId).subscribe({
      next: () => {
        this.router.navigate(['/chat', sessionId]);
      },
      error: (err: any) => {
        console.error('Failed to start session', err);
        alert('Failed to start session. Please try again.');
      }
    });
  }

  endSession(sessionId: string): void {
    if (!confirm('Are you sure you want to end this session?')) return;
    
    this.appointmentService.endSession(sessionId).subscribe({
      next: () => {
        alert('Session ended successfully');
        this.loadSessions();
      },
      error: (err: any) => {
        console.error('Failed to end session', err);
        alert('Failed to end session. Please try again.');
      }
    });
  }

  reschedule(sessionId: string): void {
    this.router.navigate(['/appointments', sessionId, 'reschedule']);
  }

  getTimeUntil(date: string, time: string): string {
    const sessionTime = new Date(date + ' ' + (time || '00:00')).getTime();
    const now = Date.now();
    const diff = sessionTime - now;

    if (diff < 0) return 'Overdue';
    
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

    if (hours > 24) {
      const days = Math.floor(hours / 24);
      return `${days}d ${hours % 24}h`;
    }

    return `${hours}h ${minutes}m`;
  }

  canStartNow(date: string, time: string): boolean {
    const sessionTime = new Date(date + ' ' + (time || '00:00')).getTime();
    const now = Date.now();
    const diff = sessionTime - now;
    
    // Can start 15 minutes before session time
    return diff <= 15 * 60 * 1000 && diff > -60 * 60 * 1000;
  }
}
