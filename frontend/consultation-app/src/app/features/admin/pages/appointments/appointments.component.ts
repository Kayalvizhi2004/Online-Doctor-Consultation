import { Component, OnInit, signal, inject } from '@angular/core';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { CommonModule } from '@angular/common';
import { AdminTableComponent } from '../../../../shared/components/admin-table/admin-table.component';
import { ToastrService } from 'ngx-toastr';

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
  private toastr = inject(ToastrService);

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

  view(row: any) { this.toastr.info('View ' + row.id); }
  showConfirm = false;
selectedAppointmentId: string | null = null;

cancel(row: any) {
  this.selectedAppointmentId = row.id;
  this.showConfirm = true;
}

confirmCancel() {
  if (!this.selectedAppointmentId) return;

  this.appointmentService.cancel(this.selectedAppointmentId).subscribe(() => {
    this.load();
    this.showConfirm = false;
    this.selectedAppointmentId = null;
  });
}
}
