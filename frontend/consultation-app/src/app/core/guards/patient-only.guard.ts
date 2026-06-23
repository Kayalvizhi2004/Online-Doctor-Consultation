import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

/**
 * Guards patient-only flows (finding/browsing doctors and booking). A doctor
 * reaching these by URL is redirected away instead of seeing a patient page.
 * (The booking API also rejects doctor bookings with 403.)
 */
export const patientOnlyGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.getUserRole() === 'Patient') return true;

  router.navigate([auth.isLoggedIn() ? '/home' : '/auth/login']);
  return false;
};