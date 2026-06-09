import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { DoctorService } from '../../../../core/services/doctor.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.scss']
})
export class BookingComponent implements OnInit {

  doctorId!: string;
  loading = false;
  slots: any[] = [];
  private fb = inject(FormBuilder);

  form = this.fb.group({
    slotId: ['', Validators.required],
    notes: ['']
  });

  constructor(
    private route: ActivatedRoute,
    private appointmentService: AppointmentService,
    private doctorService: DoctorService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.doctorId = this.route.snapshot.paramMap.get('doctorId')!;
    if (this.doctorId) this.loadDoctorSlots(this.doctorId);
  }

  private loadDoctorSlots(id: string) {
    this.loading = true;
    this.doctorService.getDoctorById(id).subscribe({
      next: (d: any) => {
        const today = new Date();
        const todayMid = new Date(today.getFullYear(), today.getMonth(), today.getDate());
        this.slots = (d?.slots || []).filter((s: any) => {
          const slotDate = s.date ? new Date(s.date) : null;
          if (!slotDate) return false;
          const slotMid = new Date(slotDate.getFullYear(), slotDate.getMonth(), slotDate.getDate());
          return !s.isBooked && slotMid >= todayMid;
        });
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  book(): void {
    if (this.form.invalid) return;

    this.loading = true;

    const payload = {
      doctorId: this.doctorId,
      slotId: this.form.value.slotId,
      notes: this.form.value.notes
    };

    this.appointmentService.book(payload).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/appointments']);
      },
      error: () => this.loading = false
    });
  }
}