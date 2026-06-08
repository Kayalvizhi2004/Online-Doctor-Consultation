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
  role: 'Patient' | 'Doctor' | 'Admin';
  specialization?: string; // Optional for Doctor
}

// This interface should match the JSON structure returned by the backend's AuthResponseDto
export interface AuthResponse {
  accessToken: string;
  user: User; 
  refreshToken?: string;
  expiresAt?: string;
}