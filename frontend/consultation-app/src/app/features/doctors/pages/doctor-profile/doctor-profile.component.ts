import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DoctorService } from '../../../../core/services/doctor.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { SlotCalendarComponent } from '../../../../shared/components/slot-calendar/slot-calendar.component';
import { RatingStarsComponent } from '../../../../shared/components/rating-stars/rating-stars.component';

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SlotCalendarComponent, RatingStarsComponent],
  templateUrl: './doctor-profile.component.html'
})
export class DoctorProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private doctorService = inject(DoctorService);
  private route = inject(ActivatedRoute);

  doctor: any = null;

  profileForm = this.fb.group({
    specialization: [''],
    bio: [''],
    consultationFee: [0]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id') || '';
    if (id) {
      this.doctorService.getDoctorById(id).subscribe((res: any) => {
        this.doctor = res || null;
        if (this.doctor) {
          this.profileForm.patchValue({
            specialization: this.doctor.specialization,
            bio: this.doctor.bio,
            consultationFee: this.doctor.consultationFee
          });
        }
      });
    }
  }

  submit() {
    this.doctorService.updateProfile(this.profileForm.value)
      .subscribe();
  }

  bookSlot(slot: any) {
    // stub for booking flow
    console.log('Book slot', slot);
  }
  
  openBooking() {
    // open booking UI - placeholder
    console.log('Open booking for doctor', this.doctor?.id);
  }
}