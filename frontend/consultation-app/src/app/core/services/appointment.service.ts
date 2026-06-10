import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private readonly api = inject(ApiService);

  book(data: any): Observable<any> {
    const payload = {
      DoctorId: data.doctorId || data.DoctorId || data.doctor || data.Doctor,
      SlotId: data.slotId || data.SlotId || data.slot || data.Slot,
      Notes: data.notes || data.Notes || ''
    };
    return this.api.post<any>('/api/appointments', payload);
  }

  getAll(status?: string): Observable<any[]> {
    const params = status ? { status } : {};
    return this.api.get<any>('/api/appointments', params).pipe(
      map((res: any) => {
        const items = res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items
          ?? (Array.isArray(res) ? res : []);
        return (items || []).map((a: any) => this.normalize(a));
      })
    );
  }

  getById(id: string): Observable<any> {
    return this.api.get<any>(`/api/appointments/${id}`).pipe(
      map((res: any) => this.normalize(res?.data ?? res?.Data ?? res ?? {}))
    );
  }

  /** Map the API's PascalCase appointment payload to the camelCase shape the UI uses. */
  private normalize(a: any) {
    if (!a) return a;
    return {
      id: a.id ?? a.Id,
      doctorId: a.doctorId ?? a.DoctorId,
      sessionId: a.sessionId ?? a.SessionId,
      doctorName: a.doctorName ?? a.DoctorName,
      specialization: a.specialization ?? a.Specialization,
      patientName: a.patientName ?? a.PatientName,
      status: a.status ?? a.Status,
      notes: a.notes ?? a.Notes,
      consultationFee: a.consultationFee ?? a.ConsultationFee,
      date: a.date ?? a.Date,
      startTime: (a.startTime ?? a.StartTime ?? '').toString().slice(0, 5),
      endTime: (a.endTime ?? a.EndTime ?? '').toString().slice(0, 5),
      createdAt: a.createdAt ?? a.CreatedAt
    };
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

  review(appointmentId: string, data: any): Observable<any> {
    return this.api.post<any>(`/api/appointments/${appointmentId}/review`, data);
  }
}
