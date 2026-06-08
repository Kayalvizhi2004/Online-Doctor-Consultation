import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/models/user.model';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  templateUrl: './dashboard-layout.component.html',
  styleUrls: ['./dashboard-layout.component.scss']
})
export class DashboardLayoutComponent implements OnInit, OnDestroy { // Implement OnDestroy
  role: string = '';
  user: User | null = null;
  unreadCount: number = 0; // Initialize unreadCount
  private authSubscription!: Subscription; // Declare authSubscription

  constructor(private router: Router, private auth: AuthService) {
  }

  ngOnInit(): void {
    this.authSubscription = this.auth.user$.subscribe(
      user => {
        this.user = user;
        this.role = user?.role || '';
        // Mock unread count for now, replace with actual service call
        this.unreadCount = 3;
      }
    );
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }
}