import { Component, OnInit, signal } from '@angular/core';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { AdminTableComponent } from '../../../../shared/components/admin-table/admin-table.component';

@Component({
  selector: 'app-admin-appointments',
  standalone: true,
  imports: [CommonModule, AdminTableComponent],
  templateUrl: './appointments.component.html',
  styleUrls: ['./appointments.component.scss']
})
export class AdminAppointmentsComponent implements OnInit {
  appointments = signal<any[]>([]);
  loading = signal(false);

  columns = [
    { title: 'Appointment ID', key: 'id' },
    { title: 'Patient', key: 'patient' },
    { title: 'Doctor', key: 'doctor' },
    { title: 'Date', key: 'date' },
    { title: 'Time', key: 'time' },
    { title: 'Status', key: 'status' }
  ];

  constructor(private appointmentService: AppointmentService) {}



  ngOnInit(): void { this.load(); }

  load() {
    this.loading.set(true);
    this.appointmentService.getAll().subscribe({ next: a => {
      const list = (a || []).map((x: any) => ({
        ...x,
        patient: x.patientName || x.patient || x.patientFullName || '',
        doctor: x.doctorName || x.doctor || '',
        date: x.date || '',
        time: (x.startTime || '') + (x.endTime ? ' - ' + x.endTime : ''),
        status: x.status || ''
      }));
      this.appointments.set(list);
      this.loading.set(false);
    }, error: () => this.loading.set(false) });
  }

  view(row: any) { alert('View ' + row.id); }
  cancel(row: any) { if (confirm('Cancel appointment ' + row.id + '?')) { this.appointmentService.cancel(row.id).subscribe(() => this.load()); } }
}
