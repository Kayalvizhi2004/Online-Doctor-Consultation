import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  private router = inject(Router);
  private auth = inject(AuthService);

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {

        console.debug('[ErrorInterceptor] HTTP error', error.status, error.url);

        if (error.status === 401) {
          // unauthorized - logout and redirect to login
          try { this.auth.logout(); } catch {}
          this.router.navigate(['/auth/login']);
        }

        if (error.status === 403) {
          this.router.navigate(['/home']);
        }

        return throwError(() => error);
      })
    );
  }
}