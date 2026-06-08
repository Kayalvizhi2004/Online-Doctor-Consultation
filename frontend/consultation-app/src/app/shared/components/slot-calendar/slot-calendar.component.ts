import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'slot-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './slot-calendar.component.html',
  styleUrls: ['./slot-calendar.component.scss']
})
export class SlotCalendarComponent {
  @Input() slots: Array<any> = [];
  @Output() select = new EventEmitter<any>();
}
