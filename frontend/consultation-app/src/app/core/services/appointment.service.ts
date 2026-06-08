import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private readonly api = inject(ApiService);

  book(data: any): Observable<any> {
    return this.api.post<any>('/api/appointments', data);
  }

  getAll(status?: string): Observable<any[]> {
    const params = status ? { status } : {};
    return this.api.get<any[]>('/api/appointments', params);
  }

  getById(id: string): Observable<any> {
    return this.api.get<any>(`/api/appointments/${id}`);
  }

  confirm(id: string): Observable<any> {
    return this.api.patch<any>(`/api/appointments/${id}/confirm`, {});
  }

  cancel(id: string): Observable<any> {
    return this.api.patch<any>(`/api/appointments/${id}/cancel`, {});
  }

  startSession(id: string): Observable<any> {
    return this.api.post<any>(`/api/appointments/${id}/session/start`, {});
  }

  endSession(id: string): Observable<any> {
    return this.api.post<any>(`/api/appointments/${id}/session/end`, {});
  }
}