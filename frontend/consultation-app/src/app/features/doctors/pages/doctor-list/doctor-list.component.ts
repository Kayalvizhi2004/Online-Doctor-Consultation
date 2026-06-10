import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
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

  // Signals so the list renders as soon as data arrives (zoneless app).
  doctors = signal<any[]>([]);
  loading = signal<boolean>(true);

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private doctorService = inject(DoctorService);

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

  ngOnInit(): void {
    this.loadDoctors();

    this.filterForm.valueChanges
      .pipe(debounceTime(300))
      .subscribe(() => this.loadDoctors());
  }

  loadDoctors(): void {
    this.loading.set(true);
    this.doctorService.getDoctors(this.filterForm.value).subscribe({
      next: (res: any) => {
        const list = res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items
          ?? (Array.isArray(res) ? res : []);
        this.doctors.set(list || []);
        this.loading.set(false);
      },
      error: () => { this.doctors.set([]); this.loading.set(false); }
    });
  }

  viewDoctor(id: string): void {
    // SPA navigation (no full page reload) so the details view loads smoothly.
    this.router.navigate(['/doctors', id]);
  }
}
