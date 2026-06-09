import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.scss']
})
export class NotificationListComponent implements OnInit {

  notifications: any[] = [];
  unreadCount = 0;
  isLoading = false;
  filterType: 'all' | 'unread' = 'all';

  constructor(private service: NotificationService) {}

  ngOnInit(): void {
    this.loadNotifications();

    // Polling every 30 seconds
    setInterval(() => this.loadNotifications(), 30000);
  }

  loadNotifications(): void {
    this.isLoading = true;
    this.service.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        
        // Sort by newest first
        this.notifications = list.sort((a: any, b: any) => {
          const dateA = new Date(a.createdAt || a.createdDate).getTime();
          const dateB = new Date(b.createdAt || b.createdDate).getTime();
          return dateB - dateA;
        });

        this.unreadCount = list.filter((n: any) => !n.isRead).length;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load notifications', err);
        this.notifications = [];
        this.unreadCount = 0;
        this.isLoading = false;
      }
    });
  }

  markRead(id: string): void {
    this.service.markRead(id).subscribe({
      next: () => {
        this.loadNotifications();
      },
      error: (err) => console.error('Failed to mark notification as read', err)
    });
  }

  markAllRead(): void {
    if (this.unreadCount === 0) return;
    
    this.service.markAll().subscribe({
      next: () => {
        this.loadNotifications();
      },
      error: (err) => console.error('Failed to mark all as read', err)
    });
  }

  deleteNotification(id: string): void {
    if (!confirm('Delete this notification?')) return;

    this.service.delete(id).subscribe({
      next: () => {
        this.loadNotifications();
      },
      error: (err: any) => console.error('Failed to delete notification', err)
    });
  }

  getNotificationType(notification: any): string {
    const type = notification.type?.toLowerCase() || '';
    if (type.includes('appointment')) return 'appointment';
    if (type.includes('message') || type.includes('chat')) return 'message';
    if (type.includes('review')) return 'review';
    if (type.includes('system')) return 'system';
    return 'general';
  }

  getNotificationIcon(type: string): string {
    switch (type) {
      case 'appointment': return '📅';
      case 'message': return '💬';
      case 'review': return '⭐';
      case 'system': return '⚙️';
      default: return '🔔';
    }
  }

  getFilteredNotifications(): any[] {
    if (this.filterType === 'unread') {
      return this.notifications.filter(n => !n.isRead);
    }
    return this.notifications;
  }

  setFilter(filter: 'all' | 'unread'): void {
    this.filterType = filter;
  }
}
