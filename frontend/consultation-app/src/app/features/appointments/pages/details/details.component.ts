import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { DoctorService } from '../../../../core/services/doctor.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-details',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './details.component.html'
})
export class DetailsComponent implements OnInit {

  appointment: any;
  private fb = inject(FormBuilder);
  reviewForm = this.fb.group({
    rating: [5],
    comment: ['']
  });
  private doctorService = inject(DoctorService);

  constructor(
    private route: ActivatedRoute,
    private service: AppointmentService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.service.getById(id).subscribe((res: any) => {
      this.appointment = res;
    });
  }

  submitReview() {
    if (!this.appointment) return;
    const id = this.appointment.id || this.appointment.id;
    const data = this.reviewForm.value;
    this.service.review(id, data).subscribe({
      next: (res: any) => {
        alert('Review submitted');
        // reload appointment
        this.service.getById(id).subscribe((res: any) => this.appointment = res);
        // If appointment contains doctorId, refresh doctor profile so ratings update
        const doctorId = this.appointment?.doctorId || this.appointment?.doctor?.id || null;
        if (doctorId) {
          this.doctorService.getDoctorById(doctorId).subscribe({ next: () => {}, error: () => {} });
        }
      },
      error: (err) => {
        console.error('Failed to submit review', err);
        alert('Failed to submit review');
      }
    });
  }
}