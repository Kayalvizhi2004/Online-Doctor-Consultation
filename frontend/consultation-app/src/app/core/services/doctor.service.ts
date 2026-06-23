import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class DoctorService {
 private readonly api = inject(ApiService);

 getDoctors(filters: any = {}): Observable<any> {
 return this.api.get<any>('/api/doctors', filters).pipe(
 map((res: any) => {
 const items = res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data?.Items ?? (Array.isArray(res) ? res : []);
 return (items || []).map((d: any) => this.normalizeDoctor(d));
 })
 );
 }

 getDoctorById(id: string): Observable<any> {
 return this.api.get<any>(`/api/doctors/${id}`).pipe(
 map((res: any) => {
 const data = res?.data ?? res?.Data ?? res ?? {};
 return this.normalizeDoctor(data);
 })
 );
 }

 /** Distinct specializations for the Find Doctors filter dropdown. */
 getSpecializations(): Observable<string[]> {
 return this.api.get<any>('/api/doctors/specializations').pipe(
 map((res: any) => {
 const list = Array.isArray(res) ? res : (res?.data ?? res?.Data ?? []);
 return (list || []).filter((s: any) => !!s);
 })
 );
 }

 updateProfile(data: any): Observable<any> {
 // Backend expects PascalCase property names (Json options use null naming policy).
 const payload = {
 Bio: data.bio ?? data.Bio ?? '',
 ConsultationFee: data.consultationFee ?? data.ConsultationFee ?? 0,
 IsAvailable: (data.isAvailable ?? data.IsAvailable) ?? false,
 Specialization: data.specialization ?? data.Specialization ?? null
 };
 return this.api.put<any>('/api/doctors/profile', payload);
 }

 addSlots(data: any): Observable<any> {
 return this.api.post<any>('/api/doctors/slots', data);
 }

 updateSlot(id: string, data: any): Observable<any> {
 return this.api.put<any>(`/api/doctors/slots/${id}`, data);
 }

 getAvailability(): Observable<any> {
 return this.api.get<any>('/api/doctors/availability');
 }

 deleteSlot(id: string): Observable<any> {
 return this.api.delete<any>(`/api/doctors/slots/${id}`);
 }

 toggleAvailability(data: any = {}): Observable<any> {
 return this.api.patch<any>('/api/doctors/availability', data);
 }

 private normalizeDoctor(d: any) {
 if (!d) return d;
 return {
 id: d.id || d.Id,
 fullName: d.fullName || d.FullName,
 specialization: d.specialization || d.Specialization,
 bio: d.bio || d.Bio,
 isAvailable: (d.isAvailable ?? d.IsAvailable) ?? false,
 consultationFee: d.consultationFee ?? d.ConsultationFee ?? 0,
 slots: (d.slots || d.Slots || []).map((s: any) => ({
 id: s.id || s.Id,
 date: s.date || s.Date,
 startTime: (s.startTime || s.StartTime || '').toString().slice(0,5),
 endTime: (s.endTime || s.EndTime || '').toString().slice(0,5),
 isBooked: s.isBooked ?? s.IsBooked ?? false
 }))
 ,
 email: d.email || d.Email,
 avatarUrl: d.avatarUrl || d.AvatarUrl || null,
 experience: d.experience ?? d.Experience,
 reviews: (d.reviews || d.Reviews || []).map((r: any) => ({
 patientName: r.patientName || r.PatientName || r.Patient?.FullName || '',
 rating: r.rating ?? r.Rating ?? 0,
 comment: r.comment || r.Comment || ''
 }))
 };
 }
}