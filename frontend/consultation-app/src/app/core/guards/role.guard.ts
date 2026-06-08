import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable()
export class RoleGuard implements CanActivate {

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data['role'];
    const userRole = this.auth.getUserRole();

    if (userRole === expectedRole) return true;
    // redirect to home or auth depending on auth state
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
    } else {
      this.router.navigate(['/home']);
    }
    return false;
  }
}