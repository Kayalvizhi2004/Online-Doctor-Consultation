import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.css']
})
export class NotificationListComponent implements OnInit {

  notifications: any[] = [];
  unreadCount = 0;

  constructor(private service: NotificationService) {}

  ngOnInit(): void {
    this.loadNotifications();

    // optional polling (enterprise fallback)
    setInterval(() => this.loadNotifications(), 30000);
  }

  loadNotifications(): void {
    this.service.getAll().subscribe((res: any) => {
      this.notifications = res;

      this.unreadCount = this.notifications.filter((n: any) => !n.isRead).length;
    });
  }

  markRead(id: string): void {
    this.service.markRead(id).subscribe(() => {
      this.loadNotifications();
    });
  }

  markAll(): void {
    this.service.markAll().subscribe(() => {
      this.loadNotifications();
    });
  }
}