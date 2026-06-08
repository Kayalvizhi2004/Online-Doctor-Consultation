import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, map } from 'rxjs';

interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  get<T>(url: string, params: any = {}): Observable<T> {
    const httpParams = new HttpParams({ fromObject: params });
    return this.http.get<ApiResponse<T>>(this.baseUrl + url, { params: httpParams }).pipe(
      map(res => (res && 'data' in res) ? (res.data as T) : (res as unknown as T))
    );
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(this.baseUrl + url, body).pipe(
      map(res => (res && 'data' in res) ? (res.data as T) : (res as unknown as T))
    );
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<ApiResponse<T>>(this.baseUrl + url, body).pipe(
      map(res => (res && 'data' in res) ? (res.data as T) : (res as unknown as T))
    );
  }

  patch<T>(url: string, body: any): Observable<T> {
    return this.http.patch<ApiResponse<T>>(this.baseUrl + url, body).pipe(
      map(res => (res && 'data' in res) ? (res.data as T) : (res as unknown as T))
    );
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(this.baseUrl + url).pipe(
      map(res => (res && 'data' in res) ? (res.data as T) : (res as unknown as T))
    );
  }
}