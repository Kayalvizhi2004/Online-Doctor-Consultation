import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

/**
 * Custom validator to ensure password and confirmPassword match.
 */
export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  return password && confirmPassword && password.value !== confirmPassword.value ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  loading = false;
  errorMessage = '';
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {}

  registerForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[0-9])(?=.*[a-z]).{8,}$/)]],
    confirmPassword: ['', [Validators.required]],
    phone: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
    role: ['Patient', Validators.required], // Patient | Doctor
    specialization: [''] // only for Doctor
  }, { validators: passwordMatchValidator });

  onRoleChange() {
    const role = this.registerForm.get('role')?.value;

    if (role === 'Doctor') {
      this.registerForm.get('specialization')?.setValidators([Validators.required]);
    } else {
      this.registerForm.get('specialization')?.clearValidators();
      this.registerForm.patchValue({ specialization: '' });
    }

    this.registerForm.get('specialization')?.updateValueAndValidity();
  }

  submit() {
    // If the form has any validation errors (required, email format, or password mismatch)
    if (this.registerForm.invalid) {
      this.errorMessage = 'Invalid input found. Please check your details.';
      // Mark all fields as touched to trigger visual validation in the HTML template
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    // Remove confirmPassword from payload before sending to POST /api/auth/register
    const { confirmPassword, ...payload } = this.registerForm.value;

    this.authService.register(payload as any).subscribe({
      next: (res: any) => {
        this.loading = false;
        // Successfully stored in DB, now redirect to login
        this.router.navigate(['/auth/login']);
      },
      error: (err: any) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || 'Registration failed';
      }
    });
  }

  onSubmit() { this.submit(); }
}