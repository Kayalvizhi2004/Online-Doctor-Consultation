import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {

  report: any;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadReport();
  }

  loadReport(): void {
    this.http.get(`${environment.apiUrl}/api/admin/reports`)
      .subscribe((res: any) => this.report = res);
  }
}