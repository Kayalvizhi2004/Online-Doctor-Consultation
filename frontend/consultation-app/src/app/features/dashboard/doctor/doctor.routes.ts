import { Routes } from '@angular/router';
import { DoctorDashboardComponent } from './doctor-dashboard.component';

// Routes for the doctor-facing dashboard (doctors-only pages).
export const DOCTOR_DASHBOARD_ROUTES: Routes = [
  { path: '', component: DoctorDashboardComponent }
];