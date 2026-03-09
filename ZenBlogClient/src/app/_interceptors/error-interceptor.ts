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
          if (!this.shouldStayOnCurrentPage(req.url)) {
            this.router.navigate(['/login']);
          }
        }
        return throwError(() => err);
      })
    );
  }

  private shouldStayOnCurrentPage(requestUrl: string): boolean {
    const currentUrl = this.router.url || '';
    const publicRouteActive = ['/verify-email', '/reset-password', '/forgot-password', '/register', '/login']
      .some(path => currentUrl.startsWith(path));

    const publicAuthRequest = [
      '/api/Auth/confirm-email',
      '/api/Auth/reset-password',
      '/api/Auth/forgot-password',
      '/api/Auth/register',
      '/api/Auth/login',
      '/api/Auth/login-google',
      '/api/Auth/resend-confirmation'
    ].some(path => requestUrl.includes(path));

    return publicRouteActive || publicAuthRequest;
  }
}
