import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { DoctorService } from '../../../core/services/doctor.service';

interface Slot {
 id: string;
 date: string; // actual calendar date, e.g. "2026-06-22"
 dayOfWeek?: string;
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

 // Signals: this app runs zoneless (no zone.js), so async updates (HTTP
 // callbacks) only refresh the view when they flow through signals.
 slots = signal<Slot[]>([]);
 form: FormGroup;
 isLoading = signal(false);
 isSubmitting = signal(false);
 editingSlotId = signal<string | null>(null);

	// We group slots by exact date (YYYY-MM-DD). The template shows the date header.

 constructor(private fb: FormBuilder, private api: ApiService, private doctorService: DoctorService) {
 this.form = this.fb.group({
 date: [new Date().toISOString().split('T')[0], Validators.required],
 startTime: ['00:00', Validators.required],
 endTime: ['00:00', Validators.required],
 capacity: [1, [Validators.required, Validators.min(1)]]
 });
 }

 ngOnInit(): void {
 this.loadSlots();
 }

 loadSlots(): void {
 this.isLoading.set(true);
 this.doctorService.getAvailability().subscribe({
 next: (res: any) => {
 const data = res?.data ?? res?.Data ?? res ?? [];
 const items = Array.isArray(data) ? data : (data?.items ?? data?.Items ?? []);
 this.slots.set(items.map((s: any) => ({
 id: s.id || s.Id,
 date: (s.date || s.Date || '').toString().slice(0, 10),
 dayOfWeek: this.weekdayOf((s.date || s.Date || '').toString().slice(0, 10)),
 startTime: (s.startTime || s.StartTime || '').toString().slice(0,5),
 endTime: (s.endTime || s.EndTime || '').toString().slice(0,5),
 isAvailable: !(s.isBooked || s.IsBooked),
 bookedCount: (s.isBooked || s.IsBooked) ? 1 : 0,
 capacity: 1
 })));
 this.isLoading.set(false);
 },
 error: (err: any) => {
 console.error('Failed to load slots', err);
 this.isLoading.set(false);
 }
 });
 }

 createSlot(): void {
 if (!this.form.valid) {
 alert('Please fill all fields correctly');
 return;
 }

 this.isSubmitting.set(true);
 const formVal = this.form.value;
 const dateStr = formVal.date;
 const payload = {
 Date: dateStr,
 StartTime: `${formVal.startTime}:00`,
 EndTime: `${formVal.endTime}:00`
 };

 if (this.editingSlotId()) {
 // Edit in place via PUT so the slot keeps its id (and isn't lost if the
 // request fails).
 this.doctorService.updateSlot(this.editingSlotId()!, payload).subscribe({
 next: () => {
 alert('Slot updated successfully');
 this.loadSlots();
 this.resetForm();
 this.isSubmitting.set(false);
 },
 error: (err: any) => {
 console.error('Failed to update slot', err);
 alert(err?.error?.Message || err?.error?.message || 'Failed to update slot');
 this.isSubmitting.set(false);
 }
 });
 } else {
 // Create new slot
 this.doctorService.addSlots(payload).subscribe({
 next: () => {
 alert('Slot created successfully');
 this.loadSlots();
 this.resetForm();
 this.isSubmitting.set(false);
 },
 error: (err: any) => {
 console.error('Failed to create slot', err);
 alert('Failed to create slot');
 this.isSubmitting.set(false);
 }
 });
 }
 }

 editSlot(slot: Slot): void {
 this.editingSlotId.set(slot.id);
 this.form.patchValue({
 date: slot.date,
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
 this.editingSlotId.set(null);
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

 /** Weekday name for a "YYYY-MM-DD" string, parsed in LOCAL time so it
 * doesn't shift a day across timezones (new Date("YYYY-MM-DD") is UTC). */
 private weekdayOf(dateStr: string): string {
 const [y, m, d] = (dateStr || '').split('-').map(Number);
 if (!y || !m || !d) return '';
 return new Date(y, m - 1, d).toLocaleDateString(undefined, { weekday: 'long' });
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
 // Format the LOCAL Y-M-D directly; toISOString() would convert to UTC and
 // can roll the date back a day (e.g. midnight local in a +offset zone).
 const y = next.getFullYear();
 const mo = String(next.getMonth() + 1).padStart(2, '0');
 const da = String(next.getDate()).padStart(2, '0');
 return `${y}-${mo}-${da}`;
 }


 getSlotStatusColor(slot: Slot): string {
 if (this.isSlotPast(slot)) return '#999999'; // Gray for past slots
 if (slot.bookedCount === slot.capacity) return '#ef4444'; // Red for fully booked
 if (slot.bookedCount > 0) return '#f59e0b'; // Yellow for partially booked
 return '#10b981'; // Green for available
 }

 isSlotPast(slot: Slot): boolean {
 if (!slot.date) return false;
 // Compare the slot's actual date + start time against now (local time).
 const slotStart = new Date(`${slot.date}T${slot.startTime || '00:00'}:00`);
 if (isNaN(slotStart.getTime())) return false;
 return slotStart.getTime() < Date.now();
 }

  /** Return the unique dates present in the slots list, sorted ascending (YYYY-MM-DD). */
  getDates(): string[] {
    const list = (this.slots() || []).map(s => s.date).filter(Boolean as any) as string[];
    const unique = Array.from(new Set(list));
    unique.sort((a, b) => a.localeCompare(b));
    return unique;
  }

  /** Return slots for a specific ISO date string (YYYY-MM-DD), sorted by start time. */
  getSlotsByDate(date: string): Slot[] {
    return (this.slots() || [])
      .filter(s => s.date === date)
      .sort((a, b) => (a.startTime || '').localeCompare(b.startTime || ''));
  }

}