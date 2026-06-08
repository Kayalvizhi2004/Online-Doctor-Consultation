import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.css'
})
export class DashboardLayoutComponent {
  role = '';
  userName = '';

  constructor(private router: Router, private auth: AuthService) {
    const raw = localStorage.getItem('user');
    try {
      const parsed = raw ? JSON.parse(raw) : null;
      const user = parsed;
      this.role = user?.role || '';
      this.userName = user?.fullName || '';
    } catch {
      this.role = '';
      this.userName = '';
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }
}