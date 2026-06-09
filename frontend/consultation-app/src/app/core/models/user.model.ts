export interface User {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  role: 'Patient' | 'Doctor' | 'Admin';
  createdAt: string;
  photoUrl?: string;
  // Doctor-specific fields
  specialization?: string;
  bio?: string;
  consultationFee?: number | string;
  isAvailable?: boolean;
  // Patient-specific fields
  dateOfBirth?: string;
  gender?: string;
  address?: string;
}