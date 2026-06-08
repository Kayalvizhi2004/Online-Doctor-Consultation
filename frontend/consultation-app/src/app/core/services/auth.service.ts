import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject, Observable, switchMap, tap, of } from 'rxjs';
import { LoginRequest, RegisterRequest } from '../models/auth.model';

interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
}

interface AuthPayload {
  accessToken: string;
  refreshToken?: string;
  expiresAt?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(ApiService);
  private readonly userSubject = new BehaviorSubject<any>(null);
  public readonly user$ = this.userSubject.asObservable();

  login(data: LoginRequest): Observable<any> {
    // backend returns { data: AuthPayload }
    return this.api.post<AuthPayload>('/api/auth/login', data).pipe(
      switchMap(payload => {
        if (!payload || !payload.accessToken) return of(null);
        this.saveTokens(payload.accessToken, payload.refreshToken || '');
        return this.api.get<any>('/api/auth/me').pipe(
          tap(profile => {
            if (profile) {
              localStorage.setItem('user', JSON.stringify(profile));
              this.userSubject.next(profile);
            }
          })
        );
      })
    );
  }

  register(data: RegisterRequest): Observable<any> {
    return this.api.post<any>('/api/auth/register', data);
  }

  me(): Observable<any> {
    return this.api.get<any>('/api/auth/me');
  }

  saveTokens(accessToken: string, refreshToken: string) {
    localStorage.setItem('token', accessToken);
    if (refreshToken) localStorage.setItem('refresh', refreshToken);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRefresh(): string | null {
    return localStorage.getItem('refresh');
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refresh');
    localStorage.removeItem('user');
    this.userSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserRole(): string {
    const user = JSON.parse(localStorage.getItem('user') || 'null');
    return user?.role || '';
  }
}