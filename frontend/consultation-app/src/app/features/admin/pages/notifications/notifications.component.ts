import { Component, OnInit, signal, inject } from '@angular/core';
import { NotificationService } from '../../../../core/services/notification.service';
import { CommonModule } from '@angular/common';
import  { ToastrService } from 'ngx-toastr';

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
  showConfirm = false;
  confirmMessage = '';
  confirmAction: (() => void) | null = null;
  private toastr = inject(ToastrService);

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void { this.load(); }

  load() { this.loading.set(true); this.notificationService.getAll().subscribe({ next: n => { this.notifications.set(n || []); this.loading.set(false); }, error: () => this.loading.set(false) }); }

  markRead(n: any) { this.notificationService.markRead(n.id).subscribe(() => this.load()); }
  markAll() { this.notificationService.markAll().subscribe(() => this.load()); }
  del(n: any): void {

  this.confirmMessage = 'Are you sure you want to delete this notification?';

  this.confirmAction = () => {

    this.notificationService.delete(n.id).subscribe({
      next: () => {
        this.load();
        this.toastr.success('Notification deleted successfully.');
        this.showConfirm = false;
      },
      error: () => {
        this.toastr.error('Failed to delete notification.');
        this.showConfirm = false;
      }
    });

  };

  this.showConfirm = true;
}
confirmYes(): void {
  if (this.confirmAction) {
    this.confirmAction();
  }
}

confirmNo(): void {
  this.showConfirm = false;
  this.confirmAction = null;
}
}