import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpErrorResponse
} from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
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
          // If the failed request was the refresh or login endpoint, do not attempt recursion
          if (req.url.includes('/api/auth/refresh') || req.url.includes('/api/auth/login')) {
            try { this.auth.logout(); } catch {}
            this.router.navigate(['/auth/login']);
            return throwError(() => error);
          }

          // Attempt to refresh token and retry original request
          return this.auth.refreshToken().pipe(
            switchMap(success => {
              if (success) {
                const token = this.auth.getToken();
                const cloned = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
                return next.handle(cloned);
              }

              try { this.auth.logout(); } catch {}
              this.router.navigate(['/auth/login']);
              return throwError(() => error);
            }),
            catchError(err2 => {
              try { this.auth.logout(); } catch {}
              this.router.navigate(['/auth/login']);
              return throwError(() => err2);
            })
          );
        }

        if (error.status === 403) {
          this.router.navigate(['/home']);
        }

        return throwError(() => error);
      })
    );
  }
}