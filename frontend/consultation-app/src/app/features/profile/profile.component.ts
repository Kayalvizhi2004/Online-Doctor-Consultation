import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ApiService);
  private auth = inject(AuthService);
  private router = inject(Router);

  form: FormGroup;
  isLoading = false;
  isSubmitting = false;
  isSaving = false;
  userRole = 'patient'; // Will be set from auth service
  successMessage = '';
  errorMessage = '';

  constructor() {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      // Doctor-specific fields
      specialization: [''],
      bio: [''],
      consultationFee: ['', Validators.min(0)],
      isAvailable: [true],
      // Patient-specific fields
      dateOfBirth: [''],
      gender: [''],
      address: ['']
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.auth.user$.subscribe(u => {
      if (u) {
        this.userRole = u.role?.toLowerCase() || 'patient';
        this.form.patchValue({
          fullName: u.fullName,
          email: u.email,
          phone: u.phone,
          specialization: u.specialization || '',
          bio: u.bio || '',
          consultationFee: u.consultationFee || '',
          isAvailable: u.isAvailable !== false,
          dateOfBirth: u.dateOfBirth || '',
          gender: u.gender || '',
          address: u.address || ''
        });
      }
      this.isLoading = false;
    });
  }

  submit(): void {
    if (!this.form.valid) {
      this.errorMessage = 'Please fill all required fields correctly';
      return;
    }

    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    const payload = this.form.value;

    this.api.put('/api/users/profile', payload).subscribe({
      next: () => {
        this.successMessage = 'Profile updated successfully!';
        this.isSubmitting = false;
        setTimeout(() => {
          this.auth.me().subscribe();
        }, 1000);
      },
      error: (err) => {
        console.error('Failed to update profile', err);
        this.errorMessage = err?.error?.message || 'Failed to update profile';
        this.isSubmitting = false;
      }
    });
  }

  toggleAvailability(): void {
    const currentValue = this.form.get('isAvailable')?.value;
    this.form.patchValue({ isAvailable: !currentValue });
  }

  getFieldError(fieldName: string): string {
    const field = this.form.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    if (field.errors['required']) return `${fieldName} is required`;
    if (field.errors['email']) return 'Invalid email format';
    if (field.errors['min']) return `${fieldName} must be greater than 0`;

    return '';
  }

  isDoctor(): boolean {
    return this.userRole === 'doctor';
  }

  isPatient(): boolean {
    return this.userRole === 'patient';
  }
}

