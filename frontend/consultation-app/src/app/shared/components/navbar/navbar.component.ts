import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'] // Assuming this is already SCSS
})
export class NavbarComponent implements OnInit {
  private auth = inject(AuthService);
  private router = inject(Router);

  isAuthenticated: boolean = false;
  user: User | null = null;
  unreadCount = 0;
  private authSubscription!: Subscription;

  ngOnInit(): void {
    this.authSubscription = this.auth.user$.subscribe(
      user => {
        this.user = user;
        this.isAuthenticated = !!user;
      }
    );
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }

  ngOnDestroy(): void { // Implement OnDestroy interface
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }
}
