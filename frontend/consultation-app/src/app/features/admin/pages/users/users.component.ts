import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../core/services/user.service';
import { ToastrService } from 'ngx-toastr';

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
  showConfirm = false;
  confirmMessage = '';
  confirmAction: (() => void) | null = null;
  private toastr = inject(ToastrService);

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
     this.confirmMessage = 'Delete user? This action cannot be undone.';

  this.confirmAction = () => {

    this.userService.delete(userId).subscribe({
      next: () => {
        this.toastr.success('User deleted successfully.');
        this.loadUsers(); // or this.load(), depending on your component
        this.showConfirm = false;
      },
      error: () => {
        this.toastr.error('Failed to delete user.');
        this.showConfirm = false;
      }
    });

  };

  this.showConfirm = true;
    this.userService.delete(userId).subscribe(() => this.loadUsers());
  }
  confirmYes(): void {
  if (this.confirmAction) {
    this.confirmAction();
  }
}

confirmNo(): void {
  this.showConfirm = false;
  this.confirmAction = null;
}
}
