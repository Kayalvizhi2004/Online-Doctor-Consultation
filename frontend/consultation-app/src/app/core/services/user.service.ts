import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(ApiService);

  getUsers(params: any = {}) {
    return this.api.get<any>('/api/admin/users', params).pipe(
      map((res: any) =>
        // support different casing from backend: items / Items / data / Data
        res?.items ?? res?.Items ?? res?.data ?? res?.Data ?? (Array.isArray(res) ? res : (res?.Items ?? res?.items ?? []))
      )
    );
  }

  getById(id: string) { return this.api.get<any>(`/api/admin/users/${id}`); }

  update(id: string, body: any) { return this.api.put<any>(`/api/admin/users/${id}`, body); }

  activate(id: string) { return this.api.patch<any>(`/api/admin/users/${id}/activate`, {}); }

  deactivate(id: string) { return this.api.patch<any>(`/api/admin/users/${id}/deactivate`, {}); }

  delete(id: string) { return this.api.delete<any>(`/api/admin/users/${id}`); }
}
