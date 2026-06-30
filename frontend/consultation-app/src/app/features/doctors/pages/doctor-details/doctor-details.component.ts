import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DoctorService } from '../../../../core/services/doctor.service';
import { AppointmentService } from '../../../../core/services/appointment.service';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-doctor-details',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './doctor-details.component.html',
  styleUrls: ['./doctor-details.component.scss']
})
export class DoctorDetailsComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private doctorService = inject(DoctorService);
  private appointmentService = inject(AppointmentService);
  private auth = inject(AuthService);

  // Signals so the view re-renders as soon as data changes.
  // (This app is zoneless, so a plain property set in subscribe() would NOT
  // trigger change detection until some other event forced it.)
  doctor = signal<any | null>(null);
  loading = signal<boolean>(true);
  error = signal<string>('');

  role = signal<string>('');
  feedback = signal<{ type: 'success' | 'error'; text: string } | null>(null);

  // Booking modal state
  bookingModalSlot = signal<any | null>(null);   // the slot being booked (modal open when set)
  notes = signal<string>('');
  modalError = signal<string>('');
  submitting = signal<boolean>(false);

  ngOnInit(): void {
    // BehaviorSubject -> fires synchronously with the current user.
    this.auth.user$.subscribe(u => this.role.set(u?.role || ''));

    const id = this.route.snapshot.paramMap.get('id')!;
    this.loadDoctor(id);
  }

  loadDoctor(id: string): void {
    this.loading.set(true);
    this.error.set('');

    this.doctorService.getDoctorById(id).subscribe({
      next: (res: any) => {
        this.doctor.set(res);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load doctor details. Please try again.');
        this.loading.set(false);
      }
    });
  }

  isPatient(): boolean {
    return this.role() === 'Patient';
  }

  getAvailableSlots(): any[] {
    const doc = this.doctor();

    if (!doc?.slots) {
      return [];
    }

    return doc.slots.filter((slot: any) => {
      return !slot.isBooked && !this.isPast(slot);
    });
  }
  /** A slot whose start time is already in the past can no longer be booked. */
  isPast(slot: any): boolean {
    if (!slot?.date || !slot?.startTime) return false;
    const start = new Date(`${slot.date}T${slot.startTime}`);
    if (isNaN(start.getTime())) return false;
    return start.getTime() < Date.now();
  }

  slotState(slot: any): 'booked' | 'past' | 'open' {
    if (slot?.isBooked) return 'booked';
    if (this.isPast(slot)) return 'past';
    return 'open';
  }

  /** Only a logged-in patient can book an open, future slot. */
  canBook(slot: any): boolean {
    const doc = this.doctor();

    return (
      this.isPatient() &&
      !!doc?.isAvailable &&
      !slot.isBooked &&
      !this.isPast(slot)
    );
  }
  // --- Booking modal ---------------------------------------------------------

  openBooking(slot: any): void {
    if (!this.canBook(slot)) return;
    this.notes.set('');
    this.modalError.set('');
    this.bookingModalSlot.set(slot);
  }

  closeBooking(): void {
    if (this.submitting()) return;   // don't close mid-request
    this.bookingModalSlot.set(null);
    this.notes.set('');
    this.modalError.set('');
  }

  confirmBooking(): void {
    const doc = this.doctor();
    const slot = this.bookingModalSlot();
    if (!doc || !slot || this.submitting()) return;

    this.submitting.set(true);
    this.modalError.set('');

    this.appointmentService
      .book({ doctorId: doc.id, slotId: slot.id, notes: this.notes().trim() })
      .subscribe({
        next: () => {
          // Reflect the booking locally so the row flips to "Booked".
          this.doctor.update((d: any) => ({
            ...d,
            slots: (d?.slots || []).map((s: any) =>
              s.id === slot.id ? { ...s, isBooked: true } : s
            )
          }));
          this.submitting.set(false);
          this.bookingModalSlot.set(null);
          this.notes.set('');
          this.feedback.set({
            type: 'success',
            text: 'Appointment booked! You can track it under My Appointments.'
          });
        },
        error: (err: any) => {
          this.submitting.set(false);
          // Keep the modal open so the user sees why and can retry/cancel.
          this.modalError.set(err?.message || 'Could not book this slot. Please try again.');
        }
      });
  }
}