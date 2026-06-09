import { Component, inject, OnInit } from '@angular/core';
import { DoctorService } from '../../../../core/services/doctor.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-doctor-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './doctor-list.component.html',
  styleUrls: ['./doctor-list.component.scss']
})
export class DoctorListComponent implements OnInit {

  doctors: any[] = [];
  loading = false;
  private fb = inject(FormBuilder);

  specializations = [
    'Cardiology',
    'Dermatology',
    'Neurology',
    'Orthopedics',
    'General Medicine'
  ];

  filterForm = this.fb.group({
    search: [''],
    specialization: ['']
  });

  constructor(
    private doctorService: DoctorService,
  ) {}

  ngOnInit(): void {
    this.loadDoctors();

    this.filterForm.valueChanges
      .pipe(debounceTime(300))
      .subscribe(() => this.loadDoctors());
  }

  loadDoctors() {
    this.loading = true;
    this.doctorService.getDoctors(this.filterForm.value)
      .subscribe({
        next: (res: any) => {
          // ApiService unwraps { data } so we may receive a paged response or an array
          this.doctors = res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? (Array.isArray(res) ? res : []);
          // defer loading flag update to avoid ExpressionChangedAfterItHasBeenCheckedError
          setTimeout(() => this.loading = false);
        },
        error: () => this.loading = false
      });
  }

  viewDoctor(id: string) {
    window.location.href = `/doctors/${id}`;
  }
}