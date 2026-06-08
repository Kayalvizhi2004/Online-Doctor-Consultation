import { Routes } from '@angular/router';
import { DoctorListComponent } from './pages/doctor-list/doctor-list.component';
import { DoctorDetailsComponent } from './pages/doctor-details/doctor-details.component';
import { DoctorProfileComponent } from './pages/doctor-profile/doctor-profile.component';

export const DOCTOR_ROUTES: Routes = [
  { path: '', component: DoctorListComponent },
  { path: ':id', component: DoctorDetailsComponent },
  { path: 'profile/edit', component: DoctorProfileComponent }
];