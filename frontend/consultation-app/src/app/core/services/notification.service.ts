import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class NotificationService {

  private auth = inject(AuthService);
  constructor(private api: ApiService) {}

  getAll() {
    // If user is not authenticated yet, avoid hitting protected endpoint
    if (!this.auth.isLoggedIn()) {
      return of([] as any[]);
    }
    return this.api.get('/api/notifications').pipe(
      map((res: any) => {
        const list = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? []);
        // API returns PascalCase; normalize to the camelCase the UI expects.
        return (list || []).map((n: any) => ({
          id: n.id ?? n.Id,
          title: n.title ?? n.Title,
          message: n.message ?? n.Message,
          type: n.type ?? n.Type,
          isRead: n.isRead ?? n.IsRead ?? false,
          createdAt: n.createdAt ?? n.CreatedAt
        }));
      })
    );
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

