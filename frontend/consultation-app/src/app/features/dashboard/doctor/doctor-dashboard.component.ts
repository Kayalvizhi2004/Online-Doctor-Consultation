import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';

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
  activeSession: any = null;

  constructor(private appointmentService: AppointmentService) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe((res: any) => {
      this.appointments = res;

      this.pending = res.filter((a: any) => a.status === 'Pending');
    });
  }

  confirm(id: string): void {
    this.appointmentService.confirm(id).subscribe(() => {
      this.loadAppointments();
    });
  }

  startSession(id: string): void {
    this.appointmentService.startSession(id).subscribe(() => {
      this.activeSession = id;
    });
  }

  endSession(id: string): void {
    this.appointmentService.endSession(id).subscribe(() => {
      this.activeSession = null;
      this.loadAppointments();
    });
  }
}