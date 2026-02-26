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
    // Do NOT attach Bearer token to auth endpoints.
    // Otherwise a stale/invalid token in localStorage can break anonymous flows like
    // /api/Auth/login-google with "JWT contains untrusted 'aud' claim".
    // (Backend always validates issuer/audience when a Bearer token is present.)
    if (req.url.includes('/api/Auth/') || req.url.endsWith('/api/Auth')) {
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
