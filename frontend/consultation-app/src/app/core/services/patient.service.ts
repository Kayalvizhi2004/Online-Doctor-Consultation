import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private api = inject(ApiService);

  getPatients(params: any = {}) {
    return this.api.get<any>('/api/admin/patients', params).pipe(
      map((res: any) => res?.items ?? res?.data ?? res ?? [])
    );
  }

  getById(id: string) { return this.api.get<any>(`/api/admin/patients/${id}`); }

  update(id: string, body: any) { return this.api.put<any>(`/api/admin/patients/${id}`, body); }

  deactivate(id: string) { return this.api.patch<any>(`/api/admin/patients/${id}/deactivate`, {}); }

  delete(id: string) { return this.api.delete<any>(`/api/admin/patients/${id}`); }
}
