import { Routes } from '@angular/router';

export const routes: Routes = [

  // Redirect root to home
  { path: '', redirectTo: 'home', pathMatch: 'full' },

  // AUTH
  {
    path: '',
    loadComponent: () =>
      import('./layouts/auth-layout/auth-layout.component')
        .then(m => m.AuthLayoutComponent),
    children: [
      {
        path: 'auth',
        loadChildren: () =>
          import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
      }
    ]
  },

  // DASHBOARD
  {
    path: '',
    loadComponent: () =>
      import('./layouts/dashboard-layout/dashboard-layout.component')
        .then(m => m.DashboardLayoutComponent),
    children: [
      {
        path: 'home',
        loadChildren: () =>
          import('./features/home/home.routes').then(m => m.HOME_ROUTES)
      },
      {
        path: 'doctors',
        loadChildren: () =>
          import('./features/doctors/doctors.routes').then(m => m.DOCTOR_ROUTES)
      },
      {
        path: 'reviews',
        loadComponent: () => import('./features/reviews/reviews.component').then(m => m.ReviewsComponent)
      },
      {
        path: 'my-reviews',
        loadComponent: () => import('./features/reviews/reviews.component').then(m => m.ReviewsComponent)
      },
      {
        path: 'appointments',
        loadChildren: () =>
          import('./features/appointments/appointments.routes').then(m => m.APPOINTMENTS_ROUTES)
      },
      {
        path: 'chat',
        loadChildren: () =>
          import('./features/chat/chat.routes').then(m => m.CHAT_ROUTES)
      },
      {
        path: 'slots',
        loadComponent: () => import('./features/dashboard/availability-slots/availability-slots.component').then(m => m.AvailabilitySlotsComponent)
      },
      {
        path: 'notifications',
        loadChildren: () =>
          import('./features/notifications/notifications.routes').then(m => m.NOTIFICATION_ROUTES)
      },
      {
        path: 'dashboard/patient',
        loadChildren: () =>
          import('./features/dashboard/patient/patient.routes').then(m => m.PATIENT_DASHBOARD_ROUTES)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent)
      },
      {
        path: 'dashboard/doctor',
        loadChildren: () =>
          import('./features/dashboard/doctor/doctor.routes').then(m => m.DOCTOR_DASHBOARD_ROUTES)
      },
    ]
  },

  { path: '**', redirectTo: 'home' }
];