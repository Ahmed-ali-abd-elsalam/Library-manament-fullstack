import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, switchMap, throwError, tap, of } from 'rxjs';

function getAccessToken(): string | null {
  return localStorage.getItem('access_token');
}

function getRefreshToken(): string | null {
  return localStorage.getItem('refresh_token');
}

function setAccessToken(token: string) {
  localStorage.setItem('access_token', token);
}

function shouldSkipAuth(req: HttpRequest<unknown>): boolean {
  const url = req.url.toLowerCase();
  return (
    url.includes('/auth/login') ||
    url.includes('/auth/register') ||
    url.includes('/auth/refresh') ||
    req.headers.has('Authorization')
  );
}

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const router = inject(Router);
  const http = inject(HttpClient);

  const token = getAccessToken();
  let authReq = req;

  // Attach Authorization header if token exists and endpoint requires it
  if (token && !shouldSkipAuth(req)) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(authReq).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        const refreshToken = getRefreshToken();

        // No refresh token → logout
        if (!refreshToken) {
          localStorage.removeItem('access_token');
          router.navigateByUrl('/login');
          return throwError(() => error);
        }

        // Attempt to refresh access token
        return http
          .post<any>('https://localhost:7205/api/Auth/refresh', {
            refresh_token: refreshToken,
          })
          .pipe(
            switchMap((res) => {
              console.log(res);

              const newAccessToken =
                res?.data?.access_Token || res?.data?.token || res?.data?.jwt || null;

              if (!newAccessToken) {
                localStorage.removeItem('access_token');
                localStorage.removeItem('refresh_token');
                router.navigateByUrl('/login');
                return throwError(() => error);
              }

              // Save new token
              setAccessToken(newAccessToken);

              // Retry the original request with the new token
              const clonedReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newAccessToken}`,
                },
              });

              return next(clonedReq);
            }),
            catchError((refreshErr) => {
              // If refresh fails, clear everything and redirect
              localStorage.removeItem('access_token');
              localStorage.removeItem('refresh_token');
              router.navigateByUrl('/login');
              return throwError(() => refreshErr);
            })
          );
      }

      // For non-401 errors, just propagate
      return throwError(() => error);
    })
  );
};
