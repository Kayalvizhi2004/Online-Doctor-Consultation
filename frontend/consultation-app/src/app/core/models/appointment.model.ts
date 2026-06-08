export interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  slotId: string;

  status: 'Pending' | 'Confirmed' | 'InProgress' | 'Completed' | 'Cancelled';

  notes: string;
  createdAt: string;
}