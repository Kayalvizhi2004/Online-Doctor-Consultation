import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { CommonModule } from '@angular/common';
import { NgIf, NgFor } from '@angular/common';


@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  users: any[] = [];
  loading = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;

    this.http.get(`${environment.apiUrl}/api/admin/users`)
      .subscribe({
        next: (res: any) => {
          this.users = res;
          this.loading = false;
        },
        error: () => this.loading = false
      });
  }

  changeRole(userId: string, role: string): void {
    this.http.patch(`${environment.apiUrl}/api/admin/users/${userId}/role`, { role })
      .subscribe(() => this.loadUsers());
  }

  toggleStatus(userId: string): void {
    this.http.patch(`${environment.apiUrl}/api/admin/users/${userId}/toggle`, {})
      .subscribe(() => this.loadUsers());
  }
}