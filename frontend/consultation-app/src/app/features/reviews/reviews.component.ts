import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { AppointmentService } from '../../core/services/appointment.service';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.scss']
})
export class ReviewsComponent implements OnInit {

  role = signal<string>('');
  loading = signal<boolean>(true);

  // Doctor view
  averageRating = signal<number>(0);
  totalReviews = signal<number>(0);
  reviews = signal<any[]>([]);

  // Patient view
  completed = signal<any[]>([]);
  private reviewedIds = signal<Set<string>>(new Set<string>());

  // Write-review modal (patient)
  modalAppt = signal<any | null>(null);
  rating = signal<number>(0);
  comment = '';
  submitting = signal<boolean>(false);
  modalError = signal<string>('');

  private route = inject(ActivatedRoute);
  private api = inject(ApiService);
  private auth = inject(AuthService);
  private appt = inject(AppointmentService);

  ngOnInit(): void {
    this.auth.user$.subscribe(u => this.role.set(u?.role || ''));

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) { this.loadDoctorReviews(idParam); return; }   // public doctor reviews
    if (this.isDoctor()) this.loadMyDoctorReviews();
    else this.loadCompletedForPatient();
  }

  isDoctor(): boolean { return this.role() === 'Doctor'; }

  // ---------- Doctor: received reviews + average ----------
  loadMyDoctorReviews(): void {
    this.loading.set(true);
    this.api.get<any>('/api/doctors/me/reviews', { pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res) => { this.applyDoctorReviews(res); this.loading.set(false); },
      error: () => { this.reviews.set([]); this.loading.set(false); }
    });
  }

  loadDoctorReviews(id: string): void {
    this.loading.set(true);
    this.api.get<any>(`/api/doctors/${id}/reviews`, { pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res) => { this.applyDoctorReviews(res); this.loading.set(false); },
      error: () => { this.reviews.set([]); this.loading.set(false); }
    });
  }

  private applyDoctorReviews(res: any): void {
    const d = res?.data ?? res?.Data ?? res ?? {};
    const items = d.items ?? d.Items ?? [];
    this.reviews.set((items || []).map((r: any) => ({
      patientName: r.patientName ?? r.PatientName ?? 'Patient',
      rating: r.rating ?? r.Rating ?? 0,
      comment: r.comment ?? r.Comment ?? '',
      createdAt: r.createdAt ?? r.CreatedAt
    })));
    this.averageRating.set(d.averageRating ?? d.AverageRating ?? 0);
    this.totalReviews.set(d.totalReviews ?? d.TotalReviews ?? (items?.length ?? 0));
  }

  // ---------- Patient: completed consultations to review ----------
  loadCompletedForPatient(): void {
    this.loading.set(true);
    this.appt.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? []);
        this.completed.set((list || []).filter((a: any) => (a.status || '').toLowerCase() === 'completed'));
        this.loading.set(false);
      },
      error: () => { this.completed.set([]); this.loading.set(false); }
    });
  }

  isReviewed(a: any): boolean { return this.reviewedIds().has(a.id); }

  openReview(a: any): void {
    this.modalAppt.set(a);
    this.rating.set(0);
    this.comment = '';
    this.modalError.set('');
  }

  closeReview(): void { if (!this.submitting()) this.modalAppt.set(null); }

  setRating(n: number): void { this.rating.set(n); }

  submitReview(): void {
    const a = this.modalAppt();
    if (!a) return;
    if (this.rating() < 1) { this.modalError.set('Please select a star rating.'); return; }

    this.submitting.set(true);
    this.appt.review(a.id, { Rating: this.rating(), Comment: this.comment.trim() }).subscribe({
      next: () => { this.markReviewed(a.id); this.submitting.set(false); this.modalAppt.set(null); },
      error: (err: any) => {
        this.submitting.set(false);
        const msg = (err?.message || 'Failed to submit review').toString();
        if (msg.toLowerCase().includes('already')) { this.markReviewed(a.id); this.modalAppt.set(null); }
        else this.modalError.set(msg);
      }
    });
  }

  private markReviewed(id: string): void {
    const s = new Set(this.reviewedIds());
    s.add(id);
    this.reviewedIds.set(s);
  }

  /** Star fill states for display: [true,true,false,...]. */
  stars(n: number): boolean[] {
    return [1, 2, 3, 4, 5].map(i => i <= Math.round(n));
  }
}
