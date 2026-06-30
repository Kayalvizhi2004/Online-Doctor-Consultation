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
  showPassword = false;
  showConfirmPassword = false;
  passwordStrength = 0;
  successMessage = '';
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

  get fullName() {
    return this.registerForm.get('fullName');
  }

  get email() {
  return this.registerForm.get('email');
  }

  get phone() {
    return this.registerForm.get('phone');
  }

  ngOnInit() {
  this.registerForm.get('password')?.valueChanges.subscribe(value => {
    this.passwordStrength = this.calculateStrength(value || '');
  });
}

calculateStrength(password: string): number {

  let score = 0;

  if (password.length >= 8) score++;
  if (/[A-Z]/.test(password)) score++;
  if (/[a-z]/.test(password)) score++;
  if (/[0-9]/.test(password)) score++;
  if (/[^A-Za-z0-9]/.test(password)) score++;

  return score;
}

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
    if (this.registerForm.invalid) {
      this.errorMessage = 'Invalid input found. Please check your details.';
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const { confirmPassword, ...payload } = this.registerForm.value;

    this.authService.register(payload as any).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.successMessage = 'Account created successfully. You can now sign in.';
        this.errorMessage = '';
        this.registerForm.reset({ role: 'Patient', specialization: '' });
        this.passwordStrength = 0;
        this.registerForm.markAsPristine();
        this.registerForm.markAsUntouched();

        setTimeout(() => {
          this.successMessage = '';
        }, 5000);
      },
      error: (err: any) => {
        this.loading = false;
        
        // Check if the backend returned structured validation errors (FluentValidation default)
        if (err.error && err.error.errors) {
          const backendErrors = err.error.errors;
          let combinedMessages: string[] = [];

          // Loop through each field error returned by .NET
          Object.keys(backendErrors).forEach((field) => {
            const messages = backendErrors[field];
            if (Array.isArray(messages)) {
              combinedMessages.push(...messages);
              
              // OPTIONAL: Push error to the specific Angular form control
              const controlName = field.charAt(0).toLowerCase() + field.slice(1); // Converts 'FullName' to 'fullName'
              const control = this.registerForm.get(controlName);
              if (control) {
                control.setErrors({ backendError: messages[0] });
              }
            }
          });

          // Join all messages with line breaks to display in the main error banner
          this.errorMessage = combinedMessages.join(' | ');
        } else if (err.error && typeof err.error === 'string') {
          // Fallback if the backend sends a plain string message
          this.errorMessage = err.error;
        } else {
          // Global fallback message
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
      }
    });
  }


  onSubmit() { this.submit(); }
}