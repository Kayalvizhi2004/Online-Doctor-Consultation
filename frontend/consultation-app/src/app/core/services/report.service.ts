import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private api = inject(ApiService);

  appointmentsPerMonth() { return this.api.get<any>('/api/reports/appointments-per-month'); }
  doctorsBySpecialization() { return this.api.get<any>('/api/reports/doctors-by-specialization'); }
  mostActiveDoctors() { return this.api.get<any>('/api/reports/most-active-doctors'); }
  mostActivePatients() { return this.api.get<any>('/api/reports/most-active-patients'); }
  revenueSummary() { return this.api.get<any>('/api/reports/revenue-summary'); }
  sessionCompletionRate() { return this.api.get<any>('/api/reports/session-completion-rate'); }
}
