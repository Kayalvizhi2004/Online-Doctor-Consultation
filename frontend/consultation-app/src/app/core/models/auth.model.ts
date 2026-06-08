import { User } from './user.model';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phone: string;
  role: 'Patient' | 'Doctor';
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}