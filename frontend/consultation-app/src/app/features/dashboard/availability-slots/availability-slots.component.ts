import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { DoctorService } from '../../../core/services/doctor.service';

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

  constructor(private fb: FormBuilder, private api: ApiService, private doctorService: DoctorService) {
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
    this.doctorService.getAvailability().subscribe({
      next: (res: any) => {
        const data = res?.data ?? res?.Data ?? res ?? [];
        const items = Array.isArray(data) ? data : (data?.items ?? data?.Items ?? []);
        this.slots = items.map((s: any) => ({
          id: s.id || s.Id,
          dayOfWeek: new Date(s.date || s.Date).toLocaleDateString(undefined, { weekday: 'long' }),
          startTime: (s.startTime || s.StartTime || '').toString().slice(0,5),
          endTime: (s.endTime || s.EndTime || '').toString().slice(0,5),
          isAvailable: !(s.isBooked || s.IsBooked),
          bookedCount: (s.isBooked || s.IsBooked) ? 1 : 0,
          capacity: 1
        }));
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
    const formVal = this.form.value;
    const dateStr = this.nextDateForWeekday(formVal.dayOfWeek);
    const payload = {
      Date: dateStr,
      StartTime: `${formVal.startTime}:00`,
      EndTime: `${formVal.endTime}:00`
    };

    if (this.editingSlotId) {
      // replace existing slot by deleting then creating new
      this.doctorService.deleteSlot(this.editingSlotId).subscribe({
        next: () => {
          this.doctorService.addSlots(payload).subscribe({
            next: () => {
              alert('Slot updated successfully');
              this.loadSlots();
              this.resetForm();
              this.isSubmitting = false;
            },
            error: (err: any) => {
              console.error('Failed to create replacement slot', err);
              alert('Failed to update slot');
              this.isSubmitting = false;
            }
          });
        },
        error: (err: any) => {
          console.error('Failed to delete slot while updating', err);
          alert('Failed to update slot');
          this.isSubmitting = false;
        }
      });
    } else {
      // Create new slot
      this.doctorService.addSlots(payload).subscribe({
        next: () => {
          alert('Slot created successfully');
          this.loadSlots();
          this.resetForm();
          this.isSubmitting = false;
        },
        error: (err: any) => {
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

    this.doctorService.deleteSlot(slotId).subscribe({
      next: () => {
        alert('Slot deleted successfully');
        this.loadSlots();
      },
      error: (err: any) => {
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
    this.doctorService.toggleAvailability({ IsAvailable: !currentStatus }).subscribe({
      next: () => {
        this.loadSlots();
      },
      error: (err: any) => {
        console.error('Failed to toggle availability', err);
      }
    });
  }

  private nextDateForWeekday(dayName: string): string {
    const dayMap: { [key: string]: number } = {
      'Sunday': 0, 'Monday': 1, 'Tuesday': 2, 'Wednesday': 3,
      'Thursday': 4, 'Friday': 5, 'Saturday': 6
    };
    const target = dayMap[dayName];
    const now = new Date();
    const diff = (target + 7 - now.getDay()) % 7 || 7; // next occurrence (not today)
    const next = new Date(now.getFullYear(), now.getMonth(), now.getDate() + diff);
    return next.toISOString().split('T')[0];
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
