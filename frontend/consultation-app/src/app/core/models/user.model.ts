export interface User {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  role: 'Patient' | 'Doctor' | 'Admin';
  createdAt: string;
}