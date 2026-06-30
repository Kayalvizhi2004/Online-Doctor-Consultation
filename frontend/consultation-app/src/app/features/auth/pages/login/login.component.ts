import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { AuthResponse } from '../../../../core/models/auth.model';
import { User } from '../../../../core/models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule], // Added CommonModule for *ngIf
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading: boolean = false; // Explicitly type and initialize
  errorMessage = '';
  showPassword = false;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ]
    }); // Removed the 'role' field as it's not part of login request
  }

  onSubmit(): void {

  if (this.loginForm.invalid) {

    this.loginForm.markAllAsTouched();

    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  this.authService.login(this.loginForm.value).subscribe({

    next: (response: AuthResponse) => {

      this.isLoading = false;

      if (!response?.user) {
        this.errorMessage =
          'Unable to load user information.';
        return;
      }

      const role =
        response.user.role?.toLowerCase();

      switch (role) {

        case 'doctor':
          this.router.navigate(['/dashboard/doctor']);
          break;

        case 'patient':
          this.router.navigate(['/dashboard/patient']);
          break;

        case 'admin':
          this.router.navigate(['/dashboard/admin']);
          break;

        default:
          this.errorMessage =
            'Unauthorized role detected.';
      }
    },

    error: (err) => {

      this.isLoading = false;

      this.errorMessage =
        err?.error?.message ||
        'Invalid email or password';
    }
  });
}
}