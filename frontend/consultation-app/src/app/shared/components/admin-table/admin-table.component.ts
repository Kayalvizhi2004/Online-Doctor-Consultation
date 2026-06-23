import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

type SignalLike<T> = (() => T) | T;

@Component({
  selector: 'app-admin-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-table.component.html',
  styleUrls: ['./admin-table.component.scss']
})
export class AdminTableComponent {
  @Input() columns: any[] = [];
  @Input() rows: SignalLike<any[]> = [];
  @Input() loading: SignalLike<boolean> = false;
  @Input() emptyMessage = 'No records found.';

  @Output() view = new EventEmitter<any>();
  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();
  @Output() toggle = new EventEmitter<any>();

  trackById(index: number, item: any) { return item.id || index; }
  get rowsValue(): any[] { return (typeof this.rows === 'function' ? (this.rows as any)() : this.rows) || []; }
  get loadingValue(): boolean { return (typeof this.loading === 'function' ? (this.loading as any)() : this.loading) || false; }
}
