import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../../core/services/api.service';

interface Slot {
  id: string;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  bookedCount: number;
  capacity: number;
}

@Component({
  selector: 'app-availability-slots',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './availability-slots.component.html',
  styleUrls: ['./availability-slots.component.scss']
})
export class AvailabilitySlotsComponent implements OnInit {

  slots: Slot[] = [];
  form: FormGroup;
  isLoading = false;
  isSubmitting = false;
  editingSlotId: string | null = null;

  daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  constructor(private fb: FormBuilder, private http: HttpClient, private api: ApiService) {
    this.form = this.fb.group({
      dayOfWeek: ['Monday', Validators.required],
      startTime: ['09:00', Validators.required],
      endTime: ['10:00', Validators.required],
      capacity: [1, [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.loadSlots();
  }

  loadSlots(): void {
    this.isLoading = true;
    this.api.get<any>('/api/doctors/availability').subscribe({
      next: (res: any) => {
        this.slots = Array.isArray(res) ? res : (res?.items ?? res?.Items ?? res?.data?.items ?? res?.Data ?? []);
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load slots', err);
        this.isLoading = false;
      }
    });
  }

  createSlot(): void {
    if (!this.form.valid) {
      alert('Please fill all fields correctly');
      return;
    }

    this.isSubmitting = true;
    const payload = this.form.value;

    if (this.editingSlotId) {
      // Update existing slot
      this.api.put<any>(`/api/doctors/availability/${this.editingSlotId}`, payload).subscribe({
        next: () => {
          alert('Slot updated successfully');
          this.loadSlots();
          this.resetForm();
          this.isSubmitting = false;
        },
        error: (err : any) => {
          console.error('Failed to update slot', err);
          alert('Failed to update slot');
          this.isSubmitting = false;
        }
      });
    } else {
      // Create new slot
      this.api.post<any>('/api/doctors/availability', payload).subscribe({
        next: () => {
          alert('Slot created successfully');
          this.loadSlots();
          this.resetForm();
          this.isSubmitting = false;
        },
        error: (err : any) => {
          console.error('Failed to create slot', err);
          alert('Failed to create slot');
          this.isSubmitting = false;
        }
      });
    }
  }

  editSlot(slot: Slot): void {
    this.editingSlotId = slot.id;
    this.form.patchValue({
      dayOfWeek: slot.dayOfWeek,
      startTime: slot.startTime,
      endTime: slot.endTime,
      capacity: slot.capacity
    });
  }

  deleteSlot(slotId: string): void {
    if (!confirm('Are you sure you want to delete this slot?')) return;

    this.api.delete<any>(`/api/doctors/availability/${slotId}`).subscribe({
      next: () => {
        alert('Slot deleted successfully');
        this.loadSlots();
      },
      error: (err : any) => {
        console.error('Failed to delete slot', err);
        alert('Failed to delete slot');
      }
    });
  }

  resetForm(): void {
    this.form.reset({
      dayOfWeek: 'Monday',
      startTime: '09:00',
      endTime: '10:00',
      capacity: 1
    });
    this.editingSlotId = null;
  }

  toggleSlotStatus(slotId: string, currentStatus: boolean): void {
    this.api.patch<any>(`/api/doctors/availability/${slotId}/toggle`, { isAvailable: !currentStatus }).subscribe({
      next: () => {
        this.loadSlots();
      },
      error: (err : any) => {
        console.error('Failed to toggle slot status', err);
      }
    });
  }

  getSlotsByDay(day: string): Slot[] {
    return this.slots.filter(s => s.dayOfWeek === day);
  }

  getSlotStatusColor(slot: Slot): string {
    if (this.isSlotPast(slot)) return '#999999'; // Gray for past slots
    if (slot.bookedCount === slot.capacity) return '#ef4444'; // Red for fully booked
    if (slot.bookedCount > 0) return '#f59e0b'; // Yellow for partially booked
    return '#10b981'; // Green for available
  }

  isSlotPast(slot: Slot): boolean {
    const now = new Date();
    const dayMap: { [key: string]: number } = {
      'Sunday': 0, 'Monday': 1, 'Tuesday': 2, 'Wednesday': 3,
      'Thursday': 4, 'Friday': 5, 'Saturday': 6
    };
    
    const slotDay = dayMap[slot.dayOfWeek];
    const today = now.getDay();
    const currentTime = now.getHours() * 60 + now.getMinutes();
    const slotTime = parseInt(slot.startTime.split(':')[0]) * 60 + parseInt(slot.startTime.split(':')[1]);
    
    if (slotDay < today) return true; // Past day
    if (slotDay === today && slotTime < currentTime) return true; // Past time today
    return false;
  }
}
