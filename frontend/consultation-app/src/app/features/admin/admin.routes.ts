import { Routes } from '@angular/router';
import { UsersComponent } from './pages/users/users.component';
import { ReportsComponent } from './pages/reports/reports.component';
import { AdminDashboardComponent } from './pages/dashboard/dashboard.component';
import { AdminDoctorsComponent } from './pages/doctors/doctors.component';
import { AdminPatientsComponent } from './pages/patients/patients.component';
import { AdminAppointmentsComponent } from './pages/appointments/appointments.component';
import { AdminNotificationsComponent } from './pages/notifications/notifications.component';
import { AuthGuard } from '../../core/guards/auth.guard';
import { RoleGuard } from '../../core/guards/role.guard';

export const ADMIN_ROUTES: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: AdminDashboardComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: 'users', component: UsersComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: 'doctors', component: AdminDoctorsComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: 'patients', component: AdminPatientsComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: 'appointments', component: AdminAppointmentsComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: 'reports', component: ReportsComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } },
  { path: 'notifications', component: AdminNotificationsComponent, canActivate: [AuthGuard, RoleGuard], data: { role: 'Admin' } }
];