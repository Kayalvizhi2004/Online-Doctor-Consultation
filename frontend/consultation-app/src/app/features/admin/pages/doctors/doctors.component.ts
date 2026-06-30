import { Component, OnInit, signal, inject } from '@angular/core';
import { DoctorService } from '../../../../core/services/doctor.service';
import { CommonModule } from '@angular/common';
import { AdminTableComponent } from '../../../../shared/components/admin-table/admin-table.component';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-doctors',
  standalone: true,
  imports: [CommonModule, AdminTableComponent],
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.scss']
})
export class AdminDoctorsComponent implements OnInit {
  doctors = signal<any[]>([]);
  loading = signal(false);
  private toastr = inject(ToastrService);
  showConfirm = false;
  confirmMessage = '';
  confirmAction: (() => void) | null = null;

  columns = [
    { title: 'Name', key: 'name' },
    { title: 'Specialization', key: 'specialization' },
    { title: 'Fee', key: 'fee' },
    // { title: 'Rating', key: 'rating' },
    { title: 'Availability', key: 'availability' },
    { title: 'Status', key: 'status' }
  ];

  constructor(private doctorService: DoctorService) {}

  ngOnInit(): void { this.load(); }

  load() {
    this.loading.set(true);
    this.doctorService.getDoctors().subscribe({ next: d => {
      const list = (d || []).map((x: any) => ({
        ...x,
        name: x.fullName || x.name || '',
        fee: x.consultationFee ?? x.fee ?? 0,
        rating: x.reviews && x.reviews.length ? (x.reviews.reduce((s: any,r: any)=> s + (r.rating||0),0) / x.reviews.length).toFixed(1) : '—',
        availability: x.isAvailable ? 'Available' : 'Offline',
        status: x.isAvailable ? 'Active' : 'Inactive'
      }));
      this.doctors.set(list);
      this.loading.set(false);
    }, error: () => this.loading.set(false) });
  }

  view(row: any) { /* implement view modal or route */ this.toastr.info('View ' + row.name); }
  edit(row: any) { /* implement edit flow */ this.toastr.info('Edit ' + row.name); }
  delete(row: any): void {

  this.confirmMessage = 'Are you sure you want to delete this doctor?';

  this.confirmAction = () => {

    // Call your backend delete API here
    // this.doctorService.delete(row.id).subscribe(() => {

    this.toastr.success('Doctor deleted successfully.');
    this.load();

    // });

    this.showConfirm = false;
  };

  this.showConfirm = true;
}

confirmYes(): void {
  if (this.confirmAction) {
    this.confirmAction();
  }
}

confirmNo(): void {
  this.showConfirm = false;
  this.confirmAction = null;
}
}
