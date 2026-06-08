export interface DoctorProfile {
  id: string;
  userId: string;
  fullName: string;
  specialization: string;
  bio: string;
  consultationFee: number;
  isAvailable: boolean;
}

export interface AvailabilitySlot {
  id: string;
  doctorId: string;
  date: string;
  startTime: string;
  endTime: string;
  isBooked: boolean;
}