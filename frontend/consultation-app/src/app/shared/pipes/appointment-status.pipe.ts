import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'appointmentStatus'
})
export class AppointmentStatusPipe implements PipeTransform {

  transform(status: string): string {

    switch (status) {
      case 'Pending': return '🟡 Pending';
      case 'Confirmed': return '🟢 Confirmed';
      case 'InProgress': return '🔵 In Progress';
      case 'Completed': return '✅ Completed';
      case 'Cancelled': return '❌ Cancelled';
      default: return status;
    }
  }
}