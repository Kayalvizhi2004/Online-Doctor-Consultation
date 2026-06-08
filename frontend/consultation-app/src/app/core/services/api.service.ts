import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, map, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

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
    return this.http.get<ApiResponse<T>>(this.baseUrl + url, { params: httpParams })
      .pipe(
        map(res => this.handleApiResponse(res)),
        catchError(error => throwError(() => error))
    );
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(this.baseUrl + url, body)
      .pipe(
        map(res => this.handleApiResponse(res)),
        catchError(error => throwError(() => error))
    );
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<ApiResponse<T>>(this.baseUrl + url, body)
      .pipe(
        map(res => this.handleApiResponse(res)),
        catchError(error => throwError(() => error))
    );
  }

  patch<T>(url: string, body: any): Observable<T> {
    return this.http.patch<ApiResponse<T>>(this.baseUrl + url, body)
      .pipe(
        map(res => this.handleApiResponse(res)),
        catchError(error => throwError(() => error))
    );
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(this.baseUrl + url)
      .pipe(
        map(res => this.handleApiResponse(res)),
        catchError(error => throwError(() => error))
    );
  }

  private handleApiResponse<T>(res: ApiResponse<T>): T {
    // Handle both camelCase and PascalCase from Backend
    const isSuccess = res.success || (res as any).Success;
    const data = res.data || (res as any).Data;
    const message = res.message || (res as any).Message;

    if (isSuccess) {
      // Return data if exists, otherwise empty object to prevent null pointer errors
      return (data !== undefined ? data : {}) as T;
    } else {
      throw new Error(message || 'An unknown error occurred');
    }
  }
}