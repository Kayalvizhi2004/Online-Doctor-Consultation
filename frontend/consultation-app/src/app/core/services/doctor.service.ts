import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  private readonly api = inject(ApiService);

  getDoctors(filters: any = {}): Observable<any> {
    return this.api.get('/api/doctors', filters);
  }

  getDoctorById(id: string): Observable<any> {
    return this.api.get<any>(`/api/doctors/${id}`);
  }

  updateProfile(data: any): Observable<any> {
    return this.api.put<any>('/api/doctors/profile', data);
  }

  addSlots(data: any): Observable<any> {
    return this.api.post<any>('/api/doctors/slots', data);
  }

  deleteSlot(id: string): Observable<any> {
    return this.api.delete<any>(`/api/doctors/slots/${id}`);
  }

  toggleAvailability(data: any = {}): Observable<any> {
    return this.api.patch<any>('/api/doctors/availability', data);
  }
}