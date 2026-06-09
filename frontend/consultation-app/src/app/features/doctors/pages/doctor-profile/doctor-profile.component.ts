import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DoctorService } from '../../../../core/services/doctor.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { SlotCalendarComponent } from '../../../../shared/components/slot-calendar/slot-calendar.component';
import { RatingStarsComponent } from '../../../../shared/components/rating-stars/rating-stars.component';
import { AuthService } from '../../../../core/services/auth.service';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SlotCalendarComponent, RatingStarsComponent],
  templateUrl: './doctor-profile.component.html',
  styleUrls: ['./doctor-profile.component.scss']
})
export class DoctorProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private doctorService = inject(DoctorService);
  private route = inject(ActivatedRoute);
  private auth = inject(AuthService);
  private appointmentService = inject(AppointmentService);
  private router = inject(Router);

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
        // support both wrapped and unwrapped responses
        const raw = res ?? {};
        const docRaw = raw.Data ?? raw.data ?? raw;

        // normalize fields (handle PascalCase coming directly from backend)
        const doctorObj: any = {
          id: docRaw.id || docRaw.Id,
          fullName: docRaw.fullName || docRaw.FullName,
          specialization: docRaw.specialization || docRaw.Specialization,
          bio: docRaw.bio || docRaw.Bio,
          consultationFee: docRaw.consultationFee ?? docRaw.ConsultationFee,
          slots: (docRaw.slots || docRaw.Slots || []).map((s: any) => ({
            id: s.id || s.Id,
            date: s.date || s.Date,
            startTime: (s.startTime || s.StartTime || '').toString().slice(0,5),
            endTime: (s.endTime || s.EndTime || '').toString().slice(0,5),
            isBooked: s.isBooked ?? s.IsBooked ?? false
          }))
        };

        this.doctor = doctorObj;
        console.debug('Loaded doctor (normalized):', this.doctor);

        if (this.doctor) {
          const today = new Date();
          const todayMid = new Date(today.getFullYear(), today.getMonth(), today.getDate());
          this.doctor.slots = (this.doctor.slots || []).filter((s: any) => {
            const slotDate = s.date ? new Date(s.date) : null;
            const isBooked = s.isBooked ?? false;
            if (!slotDate) return false;
            const slotMid = new Date(slotDate.getFullYear(), slotDate.getMonth(), slotDate.getDate());
            const upcoming = slotMid >= todayMid;
            return !isBooked && upcoming;
          });
          console.debug('Filtered slots to show:', this.doctor.slots);

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
      .subscribe({
        next: (res: any) => {
          // refresh authenticated user info so navbar and sidebar update
          this.auth.me().subscribe();
        },
        error: (err) => console.error('Failed to update profile', err)
      });
  }

  bookSlot(slot: any) {
    if (!this.doctor || !slot) return;
    const payload = {
      doctorId: this.doctor.id || this.doctor.Id || this.doctor?.id,
      slotId: slot.id || slot.Id || slot.slotId,
      notes: ''
    };
    this.appointmentService.book(payload).subscribe({
      next: (res: any) => {
        alert('Appointment booked successfully');
        this.router.navigate(['/appointments']);
      },
      error: (err) => {
        console.error('Failed to book appointment', err);
        alert('Failed to book appointment');
      }
    });
  }
  
  openBooking() {
    // open booking UI - placeholder
    console.log('Open booking for doctor', this.doctor?.id);
  }
}