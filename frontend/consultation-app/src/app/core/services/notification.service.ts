import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';
import { of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NotificationService {

  private auth = inject(AuthService);
  constructor(private api: ApiService) {}

  getAll() {
    // If user is not authenticated yet, avoid hitting protected endpoint
    if (!this.auth.isLoggedIn()) {
      return of([]);
    }
    return this.api.get('/api/notifications');
  }

  markRead(id: string) {
    if (!this.auth.isLoggedIn()) return of(null);
    return this.api.patch(`/api/notifications/${id}/read`, {});
  }

  markAll() {
    if (!this.auth.isLoggedIn()) return of(null);
    return this.api.patch(`/api/notifications/read-all`, {});
  }

  delete(id: string) {
    if (!this.auth.isLoggedIn()) return of(null);
    return this.api.delete(`/api/notifications/${id}`);
  }
}