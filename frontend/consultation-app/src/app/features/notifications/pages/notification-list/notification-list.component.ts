import { Component, OnInit, OnDestroy, inject, signal, computed } from '@angular/core';
import { NotificationService } from '../../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.scss']
})
export class NotificationListComponent implements OnInit, OnDestroy {

  // Signals so the view renders when data arrives (zoneless app).
  notifications = signal<any[]>([]);
  loading = signal<boolean>(true);
  filterType = signal<'all' | 'unread'>('all');

  unreadCount = computed(() => this.notifications().filter((n: any) => !n.isRead).length);
  filtered = computed(() =>
    this.filterType() === 'unread'
      ? this.notifications().filter((n: any) => !n.isRead)
      : this.notifications());

  private service = inject(NotificationService);
  private pollId: any;

  ngOnInit(): void {
    this.loadNotifications();
    this.pollId = setInterval(() => this.loadNotifications(), 30000);
  }

  ngOnDestroy(): void {
    if (this.pollId) clearInterval(this.pollId);
  }

  loadNotifications(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? []);
        const sorted = [...list].sort((a: any, b: any) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.notifications.set(sorted);
        this.loading.set(false);
      },
      error: () => { this.notifications.set([]); this.loading.set(false); }
    });
  }

  markRead(id: string): void {
    this.service.markRead(id).subscribe({ next: () => this.loadNotifications() });
  }

  markAllRead(): void {
    if (this.unreadCount() === 0) return;
    this.service.markAll().subscribe({ next: () => this.loadNotifications() });
  }

  deleteNotification(id: string): void {
    if (!confirm('Delete this notification?')) return;
    this.service.delete(id).subscribe({ next: () => this.loadNotifications() });
  }

  setFilter(filter: 'all' | 'unread'): void {
    this.filterType.set(filter);
  }

  getNotificationType(notification: any): string {
    const type = (notification.type || '').toString().toLowerCase();
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
}
