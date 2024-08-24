import { HttpErrorResponse, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export const authInterceptor: HttpInterceptorFn = (req, next): Observable<HttpEvent<any>> => {
  const router = inject(Router);

  return next(req.clone({
    withCredentials: true
  })).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 || 403) {
        router.navigate(['/']);
      }
      return throwError(() => error);
    })
  );
};
