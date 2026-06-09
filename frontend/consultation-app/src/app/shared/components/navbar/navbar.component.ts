import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { ChatService } from '../../../core/services/chat.service';
import { SidebarService } from '../../../core/services/sidebar.service';

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
  private chat = inject(ChatService);
  private sidebar = inject(SidebarService);

  isAuthenticated: boolean = false;
  user: User | null = null;
  unreadCount = 0;
  private subscriptions = new Subscription();

  ngOnInit(): void {
    // Monitor Authentication State
    this.subscriptions.add(
      this.auth.user$.subscribe(user => {
        this.user = user;
        this.isAuthenticated = !!user;
        if (this.isAuthenticated) {
          this.chat.getUnreadTotal().subscribe();
        }
      })
    );

    // Monitor Realtime Unread Messages
    this.subscriptions.add(
      this.chat.unreadCount$.subscribe(count => {
        this.unreadCount = count;
      })
    );
  }

  toggleSidebar() {
    this.sidebar.toggle();
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/auth/login']);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
