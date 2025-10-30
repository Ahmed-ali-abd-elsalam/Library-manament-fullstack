import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, tap, throwError } from 'rxjs';

function getAccessToken(): string | null {
  try {
    return localStorage.getItem('access_token');
  } catch {
    return null;
  }
}

function shouldSkipAuth(req: HttpRequest<unknown>): boolean {
  const url = req.url.toLowerCase();
  if (url.includes('/auth/login') || url.includes('/auth/register')) return true;
  if (req.headers.has('Authorization')) return true;
  return false;
}

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const router = inject(Router);

  const token = getAccessToken();
  let authReq = req;

  if (token && !shouldSkipAuth(req)) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    tap(() => {
      // Successful responses can be observed here if needed
    }),
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        if (error.status === 401 || error.status === 403) {
          try {
            localStorage.removeItem('access_token');
          } catch {}
          router.navigateByUrl('/login');
        }
      }
      return throwError(() => error);
    })
  );
};
