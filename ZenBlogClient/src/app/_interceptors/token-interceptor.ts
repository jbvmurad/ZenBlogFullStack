import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TokenInterceptor implements HttpInterceptor {

  private getToken(): string | null {
    // Support multiple keys (older builds / different casing)
    const raw =
      localStorage.getItem('token') ??
      localStorage.getItem('Token') ??
      sessionStorage.getItem('token') ??
      sessionStorage.getItem('Token');

    if (!raw) return null;

    // If token was JSON-stringified, remove quotes
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
    // Only skip attaching Bearer token for *anonymous* auth endpoints.
    // If we skip for every /api/Auth call, authenticated operations like:
    //   - PUT /api/Auth (update user)
    //   - DELETE /api/Auth?id=... (delete user)
    // will fail because backend requires JWT for those.
    const url = (req.url ?? '').toLowerCase();
    const isAuthController = url.includes('/api/auth');
    const anonymousAuthEndpoints = [
      '/api/auth/login',
      '/api/auth/login-google',
      '/api/auth/register',
      '/api/auth/confirm-email',
      '/api/auth/resend-confirmation',
      '/api/auth/forgot-password',
      '/api/auth/reset-password',
      '/api/auth/createtoken'
    ];

    if (isAuthController && anonymousAuthEndpoints.some(p => url.includes(p))) {
      return next.handle(req);
    }

    // If header already present, don't override
    if (req.headers.has('Authorization')) {
      return next.handle(req);
    }

    const token = this.getToken();

    if (token) {
      const clonedRequest = req.clone({
        setHeaders: {
          Authorization: 'Bearer ' + token
        }
      });
      return next.handle(clonedRequest);
    }

    return next.handle(req);
  }
}
