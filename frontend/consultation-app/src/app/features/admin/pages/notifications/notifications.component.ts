import { Component, OnInit, signal } from '@angular/core';
import { NotificationService } from '../../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class AdminNotificationsComponent implements OnInit {
  notifications = signal<any[]>([]);
  loading = signal(false);

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void { this.load(); }

  load() { this.loading.set(true); this.notificationService.getAll().subscribe({ next: n => { this.notifications.set(n || []); this.loading.set(false); }, error: () => this.loading.set(false) }); }

  markRead(n: any) { this.notificationService.markRead(n.id).subscribe(() => this.load()); }
  markAll() { this.notificationService.markAll().subscribe(() => this.load()); }
  del(n: any) { if (confirm('Delete notification?')) { this.notificationService.delete(n.id).subscribe(() => this.load()); } }
}
