import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {

  doctorId!: string;
  loading = false;
  private fb = inject(FormBuilder);

  form = this.fb.group({
    slotId: ['', Validators.required],
    notes: ['']
  });

  constructor(
    private route: ActivatedRoute,
    private appointmentService: AppointmentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.doctorId = this.route.snapshot.paramMap.get('doctorId')!;
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