import { Routes } from '@angular/router';
import { UsersComponent } from './pages/users/users.component';
import { ReportsComponent } from './pages/reports/reports.component';

export const ADMIN_ROUTES: Routes = [
  {
    path: 'users',
    component: UsersComponent
  },
  {
    path: 'reports',
    component: ReportsComponent
  }
];