import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../core/services/user.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  users = signal<any[]>([]);
  loading = signal(false);

  constructor(private userService: UserService) {}

  ngOnInit(): void { this.loadUsers(); }

  loadUsers(): void {
    this.loading.set(true);
    this.userService.getUsers().subscribe({
      next: (res: any) => { this.users.set(res || []); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  activate(userId: string) { this.userService.activate(userId).subscribe(() => this.loadUsers()); }
  deactivate(userId: string) { this.userService.deactivate(userId).subscribe(() => this.loadUsers()); }

  deleteUser(userId: string) {
    if (!confirm('Delete user? This action cannot be undone.')) return;
    this.userService.delete(userId).subscribe(() => this.loadUsers());
  }
}
