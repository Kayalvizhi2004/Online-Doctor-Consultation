import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = inject(ApiService);

  getKpis(): Observable<any> {
    return this.api.get<any>('/api/admin/kpis').pipe(
      map((res: any) => res)
    );
  }

  // Users
  getUsers(params: any = {}) {
    return this.api.get<any>('/api/admin/users', params).pipe(
      map((res: any) => res?.items ?? res?.data ?? res ?? [])
    );
  }

  getUserById(id: string) { return this.api.get<any>(`/api/admin/users/${id}`); }

  updateUser(id: string, data: any) { return this.api.put<any>(`/api/admin/users/${id}`, data); }

  toggleUserStatus(id: string) { return this.api.patch<any>(`/api/admin/users/${id}/toggle`, {}); }

  deleteUser(id: string) { return this.api.delete<any>(`/api/admin/users/${id}`); }

  // Notifications
  getNotifications() { return this.api.get<any>('/api/notifications').pipe(map((r:any)=>r)); }
  markReadNotification(id: string) { return this.api.patch<any>(`/api/notifications/${id}/read`, {}); }
  markAllNotifications() { return this.api.patch<any>('/api/notifications/read-all', {}); }
  deleteNotification(id: string) { return this.api.delete<any>(`/api/notifications/${id}`); }
}
