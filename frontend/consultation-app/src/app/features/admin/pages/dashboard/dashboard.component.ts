import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../../../core/services/admin.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  kpis: any = {};
  loading = false;

  constructor(private admin: AdminService) {}

  ngOnInit(): void {
    this.load();
  }

  load() {
    this.loading = true;
    this.admin.getKpis().subscribe(k => { this.kpis = k; this.loading = false; });
  }
}
