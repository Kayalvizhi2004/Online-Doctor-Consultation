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

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    }); // Removed the 'role' field as it's not part of login request
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.authService.login(this.loginForm.value as any).subscribe({
        next: (response: AuthResponse) => { // Expect AuthResponse object
          this.isLoading = false;
          
          if (!response.user) {
            this.errorMessage = 'User profile not found. Please try again.';
            return;
          }

          const role = response.user.role;
          const normalizedRole = role?.toLowerCase();

          if (normalizedRole === 'doctor') {
            console.log('Navigating to Doctor Dashboard');
            this.router.navigate(['/dashboard/doctor']);
          } else if (normalizedRole === 'patient') {
            console.log('Navigating to Patient Dashboard');
            this.router.navigate(['/dashboard/patient']);
          } else if (normalizedRole === 'admin') {
            console.log('Navigating to Admin Dashboard');
            this.router.navigate(['/dashboard/admin']);
          }
        },
        error: (err:any) => {
          this.errorMessage = err.error?.message || 'Login failed';
          this.isLoading = false;
        }
      });
    }
  }
}