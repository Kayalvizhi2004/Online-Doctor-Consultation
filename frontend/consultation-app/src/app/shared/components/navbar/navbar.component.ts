import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  private auth = inject(AuthService);
  private router = inject(Router);

  isAuthenticated = false;
  user: any = null;
  unreadCount = 0;

  ngOnInit(): void {
    this.isAuthenticated = this.auth.isLoggedIn();
    try { this.user = JSON.parse(localStorage.getItem('user') || 'null'); } catch { this.user = null; }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }
}
