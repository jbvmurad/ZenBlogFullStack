import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

declare const alertify: any;

@Injectable({
  providedIn: 'root'
})
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status === 401) {
          localStorage.removeItem('token');
          const msg = err?.error?.message ?? err?.error?.Message ?? 'Yetkin yok veya oturum süren dolmuş. Lütfen tekrar giriş yap.';
          if (typeof alertify !== 'undefined') {
            alertify.error(msg);
          }
          if (!this.router.url.includes('/login')) {
            this.router.navigate(['/login']);
          }
        }
        return throwError(() => err);
      })
    );
  }
}
