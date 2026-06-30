import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpErrorResponse
} from '@angular/common/http';
import {  BehaviorSubject, throwError } from 'rxjs';
import { filter, take, switchMap, catchError, finalize } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  private router = inject(Router);
  private auth = inject(AuthService);
  private isRefreshing = false;
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);

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
          if (!this.isRefreshing) {

            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);

            return this.auth.refreshToken().pipe(

              switchMap(success => {

                if (!success) {
                  this.auth.logout();
                  this.router.navigate(['/auth/login']);
                  return throwError(() => error);
              }

            const token = this.auth.getToken();

            this.refreshTokenSubject.next(token);

            const cloned = req.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });

            return next.handle(cloned);
        }),

        finalize(() => {
            this.isRefreshing = false;
        }),

        catchError(err => {
            this.auth.logout();
            this.router.navigate(['/auth/login']);
            return throwError(() => err);
        })
    );
}

return this.refreshTokenSubject.pipe(

    filter(token => token !== null),

    take(1),

    switchMap(token => {

        const cloned = req.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });

        return next.handle(cloned);
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