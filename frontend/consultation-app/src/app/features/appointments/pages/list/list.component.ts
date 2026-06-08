import { Component, OnInit, inject } from '@angular/core';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './list.component.html'
})
export class ListComponent implements OnInit {

  appointments: any[] = [];
  private service = inject(AppointmentService);
  private router = inject(Router);

  ngOnInit(): void {
    this.load();
  }

  load(status?: string): void {
    this.service.getAll(status).subscribe((res: any) => {
      this.appointments = res || [];
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
    if (!a || a.status === 'Cancelled') return false;
    const slot = new Date(a.date + ' ' + (a.startTime || '00:00'));
    return (slot.getTime() - Date.now()) > (60 * 60 * 1000);
  }

  cancel(id: string) {
    this.service.cancel(id).subscribe(() => this.load());
  }

  canJoin(a: any): boolean {
    return a && (a.status === 'Confirmed');
  }

  join(sessionId: string) {
    if (!sessionId) return;
    this.router.navigate(['/consultation', sessionId]);
  }
}