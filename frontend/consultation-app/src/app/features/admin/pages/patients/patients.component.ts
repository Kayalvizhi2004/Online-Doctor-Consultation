import { Component, OnInit, signal } from '@angular/core';
import { PatientService } from '../../../../core/services/patient.service';
import { CommonModule } from '@angular/common';
import { AdminTableComponent } from '../../../../shared/components/admin-table/admin-table.component';

@Component({
  selector: 'app-admin-patients',
  standalone: true,
  imports: [CommonModule, AdminTableComponent],
  templateUrl: './patients.component.html',
  styleUrls: ['./patients.component.scss']
})
export class AdminPatientsComponent implements OnInit {
  patients = signal<any[]>([]);
  loading = signal(false);

  columns = [
    { title: 'Patient Name', key: 'name' },
    { title: 'Email', key: 'email' },
    { title: 'Phone', key: 'phone' },
    { title: 'Total Appointments', key: 'totalAppointments' },
    { title: 'Status', key: 'status' }
  ];

  constructor(private patientService: PatientService) {}



  ngOnInit(): void { this.load(); }

  load() {
    this.loading.set(true);
    this.patientService.getPatients().subscribe({ next: p => {
      const src = Array.isArray(p) ? p : Array.isArray(p?.items) ? p.items : Array.isArray(p?.data) ? p.data : Array.isArray(p?.data?.items) ? p.data.items : [];
      const list = (src || []).map((x: any) => ({
        ...x,
        name: x.fullName || x.fullNameText || x.name || '',
        email: x.email || '',
        phone: x.phone || x.phoneNumber || '',
        totalAppointments: x.totalAppointments ?? x.TotalAppointments ?? x.TotalAppointmentsCount ?? 0,
        status: x.status || x.Status || 'Active'
      }));
      this.patients.set(list);
      this.loading.set(false);
    }, error: () => this.loading.set(false) });
  }

  view(row: any) { alert('View ' + row.name); }
  edit(row: any) { alert('Edit ' + row.name); }
}
