import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { DoctorService } from '../../../../core/services/doctor.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  doctors: any[] = [];
  loading = false;
  private fb = inject(FormBuilder);
  private doctorService = inject(DoctorService);
  specializations = [
    'Cardiology',
    'Dermatology',
    'Neurology',
    'Orthopedics',
    'General Medicine'
  ];

  searchForm = this.fb.group({
    search: [''],
    specialization: ['']
  });

  constructor() {}

  ngOnInit(): void {
    this.setupSearch();
    this.loadDoctors();
  }

  setupSearch() {
    this.searchForm.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.loadDoctors();
      });
  }

  loadDoctors() {
    this.loading = true;
    const { search, specialization } = this.searchForm.value;
    this.doctorService.getDoctors({ search, specialization }).subscribe({
      next: (res: any) => {
        this.doctors = res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? (Array.isArray(res) ? res : []);
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  viewDoctor(id: string) {
    window.location.href = `/doctors/${id}`;
  }
}