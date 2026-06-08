import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  loading = false;
  errorMessage = '';
  private fb = inject(FormBuilder);

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  registerForm = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[0-9])(?=.*[a-z]).{8,}$/)]],
    confirmPassword: ['', [Validators.required]],
    phone: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
    role: ['Patient', Validators.required], // Patient | Doctor
    specialization: [''] // only for Doctor
  });

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
    if (this.registerForm.invalid) return;

    // password match validation
    const pw = this.registerForm.get('password')?.value;
    const cpw = this.registerForm.get('confirmPassword')?.value;

    if (pw !== cpw) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.authService.register(this.registerForm.value as any).subscribe({
      next: (res: any) => {
        this.loading = false;
        // Navigate to auth login route
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