import { Component, inject } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private fb = inject(FormBuilder);

  loading = false;
  error = '';

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[0-9])(?=.*[a-z]).{8,}$/)]],
    role: ['Patient']
  });

  onSubmit() {
    this.error = '';
    if (this.loginForm.invalid) return;

    this.loading = true;

    this.auth.login(this.loginForm.value as any).subscribe({
      next: () => {
        this.loading = false;
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        const role = user?.role || '';
        if (role === 'Doctor') this.router.navigate(['/dashboard/doctor']);
        else if (role === 'Patient') this.router.navigate(['/dashboard/patient']);
        else this.router.navigate(['/home']);
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err?.error?.message || 'Login failed. Check credentials.';
      }
    });
  }
}