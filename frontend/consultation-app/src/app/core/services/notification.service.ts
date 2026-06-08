import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class NotificationService {

  constructor(private api: ApiService) {}

  getAll() {
    return this.api.get('/api/notifications');
  }

  markRead(id: string) {
    return this.api.patch(`/api/notifications/${id}/read`, {});
  }

  markAll() {
    return this.api.patch(`/api/notifications/read-all`, {});
  }
}