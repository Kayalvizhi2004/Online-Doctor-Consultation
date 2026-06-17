import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { DoctorService } from '../../../core/services/doctor.service';

interface Slot {
  id: string;
  date: string;
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

  constructor(private fb: FormBuilder, private api: ApiService, private doctorService: DoctorService) {
    this.form = this.fb.group({
      date: [new Date().toISOString().split('T')[0], Validators.required],
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
        
        this.slots = items
          .map((s: any) => ({
            id: s.id || s.Id,
            date: s.date || s.Date,
            startTime: (s.startTime || s.StartTime || '').toString().slice(0, 5),
            endTime: (s.endTime || s.EndTime || '').toString().slice(0, 5),
            isAvailable: !(s.isBooked || s.IsBooked),
            bookedCount: (s.isBooked || s.IsBooked) ? 1 : 0,
            capacity: 1
          }))
          // Filter for unbooked (not yet booked) and upcoming (time not passed) slots
          .filter((s: Slot) => s.isAvailable && !this.isSlotPast(s))
          // Sort by date and time
          .sort((a: Slot, b: Slot) => a.date.localeCompare(b.date) || a.startTime.localeCompare(b.startTime));

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
    const payload = {
      Date: formVal.date,
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
      date: slot.date.split('T')[0],
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
      date: new Date().toISOString().split('T')[0],
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

  getSlotStatusColor(slot: Slot): string {
    if (this.isSlotPast(slot)) return '#999999'; // Gray for past slots
    if (slot.bookedCount === slot.capacity) return '#ef4444'; // Red for fully booked
    if (slot.bookedCount > 0) return '#f59e0b'; // Yellow for partially booked
    return '#10b981'; // Green for available
  }

  isSlotPast(slot: Slot): boolean {
    const now = new Date();
    const slotDate = new Date(slot.date);
    slotDate.setHours(0,0,0,0);
    const today = new Date();
    today.setHours(0,0,0,0);

    if (slotDate < today) return true;
    
    const currentTime = now.getHours() * 60 + now.getMinutes();
    const slotTime = parseInt(slot.startTime.split(':')[0]) * 60 + parseInt(slot.startTime.split(':')[1]);
    if (slotDate.getTime() === today.getTime() && slotTime < currentTime) return true;

    return false;
  }
}
