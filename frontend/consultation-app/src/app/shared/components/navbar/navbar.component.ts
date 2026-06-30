import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, OnDestroy, signal, HostListener } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { Subscription, interval } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/user.model';
import { ChatService } from '../../../core/services/chat.service';
import { SidebarService } from '../../../core/services/sidebar.service';
import { NotificationService } from '../../../core/services/notification.service';

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
  private notifications = inject(NotificationService);

  isAuthenticated: boolean = false;
  user: User | null = null;
  // Signal so the badge actually re-renders — this app is zoneless, so a plain
  // property mutated inside an RxJS subscription would never update the view.
  unreadCount = signal(0);
  unreadNotifications = signal(0);
  private subscriptions = new Subscription();

  private loadUnreadNotifications(): void {
    this.notifications.getAll().subscribe(list => {
      const unread = (list || []).filter((n: any) => !n.isRead).length;
      this.unreadNotifications.set(unread);
    });
  }

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

    // Reflect the unread total in the badge.
    this.subscriptions.add(
      this.chat.unreadCount$.subscribe(count => {
        this.unreadCount.set(count);
      })
    );

    // Notification unread count
    this.loadUnreadNotifications();

    this.subscriptions.add(
    this.notifications.notificationRefresh$.subscribe(() => {
      this.loadUnreadNotifications();
    })
  );
    // Poll so the badge updates when new notifications arrive (e.g. a doctor
    // confirms an appointment) without needing a page reload.
    this.subscriptions.add(
      interval(20000).subscribe(() => {
        if (this.isAuthenticated) {
          this.chat.getUnreadTotal().subscribe();
          this.loadUnreadNotifications();
        }
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

  isProfileMenuOpen = false;

  toggleProfileMenu() {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {

    const target = event.target as HTMLElement;

    if (!target.closest('.user-menu')) {
        this.isProfileMenuOpen = false;
   }
  }

  get dashboardRoute(): string {

  switch (this.user?.role) {

    case 'Doctor':
      return '/dashboard/doctor';

    case 'Patient':
      return '/dashboard/patient';

    case 'Admin':
      return '/admin';

    default:
      return '/';
    }
  }

  get profileRoute(): string {

  switch (this.user?.role) {

    case 'Doctor':
      return '/profile';

    case 'Patient':
      return '/profile';

    case 'Admin':
      return '/profile';

    default:
      return '/';
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}