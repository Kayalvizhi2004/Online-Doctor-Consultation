import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { BehaviorSubject, Observable, tap, switchMap, of, map } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.model';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly api = inject(ApiService);
  private readonly userSubject = new BehaviorSubject<User | null>(null);
  public readonly user$ = this.userSubject.asObservable();

  constructor() {
    this.loadUserFromLocalStorage();
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('/api/auth/login', data).pipe(
      switchMap(payload => {
        // Handle property name variations (AccessToken vs accessToken)
        const token = payload.accessToken || (payload as any).AccessToken;
        const refreshToken = payload.refreshToken || (payload as any).RefreshToken;
        const user = payload.user || (payload as any).User;

        if (token) this.saveTokens(token, refreshToken || '');

        // Robust user detection: if payload is the user, or contains User/user
        let userObj = user || ((payload as any).role || (payload as any).Role ? payload : null);

        if (!userObj) {
          return this.me().pipe(map(u => ({ ...payload, user: this.normalizeUser(u), accessToken: token })));
        }

        userObj = this.normalizeUser(userObj);
        localStorage.setItem('user', JSON.stringify(userObj));
        this.userSubject.next(userObj);
        return of({ ...payload, user: userObj, accessToken: token });
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('/api/auth/register', data).pipe(
      tap(payload => {
        const token = payload.accessToken || (payload as any).AccessToken;
        const user = this.normalizeUser(payload.user || (payload as any).User);
        if (token && user) {
          this.saveTokens(token, payload.refreshToken || '');
          localStorage.setItem('user', JSON.stringify(user));
          this.userSubject.next(user);
        }
      })
    );
  }

  me(): Observable<User> {
    return this.api.get<User>('/api/auth/me').pipe(
      map(u => this.normalizeUser(u)),
      tap(user => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.userSubject.next(user);
        }
      })
    );
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

  isLoggedIn(): boolean { // Check if userSubject has a value
    return !!this.userSubject.value;
  }

  getUserRole(): string {
    return this.userSubject.value?.role || '';
  }

  private loadUserFromLocalStorage(): void {
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      try {
        this.userSubject.next(JSON.parse(storedUser));
      } catch (e) { console.error('Error parsing user from localStorage:', e); }
    }
  }

  private normalizeUser(u: any): User {
    if (!u) return u;
    return {
      id: u.id || u.Id,
      fullName: u.fullName || u.FullName,
      email: u.email || u.Email,
      phone: u.phone || u.Phone,
      role: u.role || u.Role,
      createdAt: u.createdAt || u.CreatedAt,
      specialization: u.specialization || u.Specialization,
      bio: u.bio || u.Bio,
      consultationFee: u.consultationFee ?? u.ConsultationFee,
      isAvailable: u.isAvailable ?? u.IsAvailable,
      dateOfBirth: u.dateOfBirth || u.DateOfBirth,
      gender: u.gender || u.Gender,
      address: u.address || u.Address,
      // Use locally stored photo if available
      photoUrl: localStorage.getItem('photoUrl') || 'assets/default-avatar.svg'
    };
  }
}