import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TokenInterceptor implements HttpInterceptor {
  private anonymousAuthPaths = [
    '/api/Auth/login',
    '/api/Auth/login-google',
    '/api/Auth/register',
    '/api/Auth/confirm-email',
    '/api/Auth/resend-confirmation',
    '/api/Auth/forgot-password',
    '/api/Auth/reset-password',
    '/api/Auth/createtoken'
  ];

  private getToken(): string | null {
    const raw =
      localStorage.getItem('token') ??
      localStorage.getItem('Token') ??
      sessionStorage.getItem('token') ??
      sessionStorage.getItem('Token');

    if (!raw) return null;

    const trimmed = raw.trim();
    if (trimmed.startsWith('"') && trimmed.endsWith('"')) {
      try {
        return JSON.parse(trimmed);
      } catch {
        return trimmed.replace(/^"|"$/g, '');
      }
    }

    return trimmed;
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.anonymousAuthPaths.some(path => req.url.includes(path))) {
      return next.handle(req);
    }

    if (req.headers.has('Authorization')) {
      return next.handle(req);
    }

    const token = this.getToken();
    if (!token) {
      return next.handle(req);
    }

    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: 'Bearer ' + token
      }
    });

    return next.handle(clonedRequest);
  }
}
