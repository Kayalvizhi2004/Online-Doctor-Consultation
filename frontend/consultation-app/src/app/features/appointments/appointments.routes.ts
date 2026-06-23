import { Routes } from '@angular/router';
import { BookingComponent } from './pages/booking/booking.component';
import { ListComponent } from './pages/list/list.component';
import { DetailsComponent } from './pages/details/details.component';
import { patientOnlyGuard } from '../../core/guards/patient-only.guard';

export const APPOINTMENTS_ROUTES: Routes = [
  { path: '', component: ListComponent },
  { path: 'booking/:doctorId', component: BookingComponent, canActivate: [patientOnlyGuard] },
  { path: ':id', component: DetailsComponent }
];