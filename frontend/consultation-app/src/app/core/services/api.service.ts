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
    // If backend returns the ApiResponse wrapper (success / data), use it.
    const maybe = res as any;
    if (maybe && (maybe.hasOwnProperty('success') || maybe.hasOwnProperty('Success') || maybe.hasOwnProperty('Data') || maybe.hasOwnProperty('data'))) {
      const isSuccess = maybe.success || maybe.Success;
      const data = maybe.data !== undefined ? maybe.data : (maybe.Data !== undefined ? maybe.Data : undefined);
      const message = maybe.message || maybe.Message;

      if (isSuccess || isSuccess === undefined) {
        return (data !== undefined ? data : ({} as T)) as T;
      } else {
        throw new Error(message || 'An unknown error occurred');
      }
    }

    // Not an ApiResponse wrapper — return raw payload (arrays or plain objects)
    return (res as unknown) as T;
  }
}