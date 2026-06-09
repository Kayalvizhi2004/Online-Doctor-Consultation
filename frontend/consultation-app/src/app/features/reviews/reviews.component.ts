import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../core/services/api.service';

interface Review {
  id: string;
  patientName: string;
  patientImage?: string;
  rating: number;
  reviewText: string;
  consultationDate: string;
  createdDate: string;
}

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reviews.component.html',
  styleUrls: ['./reviews.component.scss']
})
export class ReviewsComponent implements OnInit {

  reviews: Review[] = [];
  isLoading = false;
  averageRating = 0;
  totalReviews = 0;
  ratingDistribution = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 };
  
  // Pagination
  currentPage = 1;
  itemsPerPage = 5;
  totalPages = 1;

  constructor(
    private route: ActivatedRoute,
    private api: ApiService
  ) {}

  ngOnInit(): void {
    const doctorId = this.route.snapshot.paramMap.get('id');
    if (doctorId) {
      this.loadReviewsForDoctor(doctorId);
      return;
    }

    // No doctor id in route — try to load current user and fetch their doctor reviews (for /my-reviews)
    this.api.get<any>('/api/auth/me').subscribe({
      next: (profile: any) => {
        const id = profile?.id || profile?.Id;
        const role = profile?.role || profile?.Role;
        if (id && role && role.toLowerCase() === 'doctor') {
          this.loadReviewsForDoctor(id);
        } else {
          // Not a doctor or no id — show empty state
          this.reviews = [];
          this.calculateStats();
        }
      },
      error: (err) => {
        console.error('Failed to load profile', err);
        this.reviews = [];
        this.calculateStats();
      }
    });
  }

  loadReviewsForDoctor(doctorId: string): void {
    this.isLoading = true;
    this.api.get<any>(`/api/doctors/${doctorId}/reviews`, { pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.data ?? res?.Data ?? []);
        this.reviews = list.sort((a: any, b: any) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
        this.calculateStats();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load reviews', err);
        this.isLoading = false;
      }
    });
  }

  calculateStats(): void {
    this.totalReviews = this.reviews.length;
    
    if (this.totalReviews === 0) {
      this.averageRating = 0;
      return;
    }

    const sum = this.reviews.reduce((acc, r) => acc + r.rating, 0);
    this.averageRating = sum / this.totalReviews;

    // Reset distribution
    this.ratingDistribution = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 };

    // Count ratings
    this.reviews.forEach(r => {
      const ratingKey = Math.round(r.rating) as keyof typeof this.ratingDistribution;
      if (ratingKey in this.ratingDistribution) {
        this.ratingDistribution[ratingKey]++;
      }
    });

    this.updatePagination();
  }

  getRatingPercentage(rating: number): number {
    if (this.totalReviews === 0) return 0;
    return (this.ratingDistribution[rating as keyof typeof this.ratingDistribution] / this.totalReviews) * 100;
  }

  generateStars(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i < Math.round(rating) ? 1 : 0);
  }

  getRatingColor(rating: number): string {
    if (rating >= 4) return '#10b981';
    if (rating >= 3) return '#f59e0b';
    return '#ef4444';
  }

  getRatingCount(rating: number): number {
    return this.ratingDistribution[rating as keyof typeof this.ratingDistribution] || 0;
  }

  getPaginatedReviews(): Review[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.reviews.slice(start, start + this.itemsPerPage);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      window.scrollTo(0, 0);
    }
  }

  nextPage(): void {
    this.goToPage(this.currentPage + 1);
  }

  prevPage(): void {
    this.goToPage(this.currentPage - 1);
  }

  updatePagination(): void {
    this.totalPages = Math.ceil(this.reviews.length / this.itemsPerPage);
    this.currentPage = 1;
  }
}