import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, Observable, catchError, map, of, tap } from 'rxjs';
import { UserDto } from '../_models/userDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authBaseUrl = '/api/Auth';
  private userRoleBaseUrl = '/api/UserRole';
  private cachedUserKey = 'currentUserProfile';
  private cachedAdminKey = 'currentUserIsAdmin';

  decodedToken: any;
  jwtHelper = new JwtHelperService();

  private currentUserSubject = new BehaviorSubject<UserDto | null>(this.readCachedUser());
  currentUser$ = this.currentUserSubject.asObservable();

  private adminStatusSubject = new BehaviorSubject<boolean>(this.readCachedAdmin());
  isAdmin$ = this.adminStatusSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  login(model: any) {
    return this.http.post<any>(`${this.authBaseUrl}/login`, model);
  }

  loginWithGoogle(idToken: string) {
    return this.http.post<any>(`${this.authBaseUrl}/login-google`, { idToken });
  }

  register(model: any) {
    return this.http.post<any>(`${this.authBaseUrl}/register`, model);
  }

  confirmEmail(userId: string, token: string) {
    return this.http.post<any>(`${this.authBaseUrl}/confirm-email`, { userId, token });
  }

  forgotPassword(model: any) {
    return this.http.post<any>(`${this.authBaseUrl}/forgot-password`, model);
  }

  resetPassword(model: any) {
    return this.http.post<any>(`${this.authBaseUrl}/reset-password`, model);
  }

  update(model: {
    id: string;
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    password?: string;
    imageUrl?: string;
  }) {
    return this.http.put<any>(this.authBaseUrl, {
      id: model.id,
      fullName: model.fullName,
      email: model.email,
      phoneNumber: model.phoneNumber,
      password: model.password,
      imageUrl: model.imageUrl
    }).pipe(
      tap(() => this.refreshCurrentUser().subscribe())
    );
  }

  updateWithMedia(model: {
    id: string;
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    password?: string;
    imageUrl?: string;
  }, imageFile?: File | null) {
    const form = new FormData();
    if (imageFile) form.append('Image', imageFile);

    const qs: string[] = [`id=${encodeURIComponent(model.id ?? '')}`];
    if (model.fullName != null) qs.push(`FullName=${encodeURIComponent(model.fullName)}`);
    if (model.email != null) qs.push(`Email=${encodeURIComponent(model.email)}`);
    if (model.phoneNumber != null) qs.push(`PhoneNumber=${encodeURIComponent(model.phoneNumber)}`);
    if (model.password != null) qs.push(`Password=${encodeURIComponent(model.password)}`);
    if (model.imageUrl != null) qs.push(`ImageUrl=${encodeURIComponent(model.imageUrl)}`);

    return this.http.put<any>(`${this.authBaseUrl}/with-media?${qs.join('&')}`, form).pipe(
      tap(() => this.refreshCurrentUser().subscribe())
    );
  }

  deleteUser(id: string) {
    return this.http.delete<any>(`${this.authBaseUrl}?id=${encodeURIComponent(id)}`).pipe(
      tap(() => this.clearSession())
    );
  }

  getCurrentUser(forceRefresh: boolean = false): Observable<UserDto | null> {
    const cached = this.currentUserSubject.value;
    if (!forceRefresh && cached) return of(cached);
    return this.refreshCurrentUser();
  }

  refreshCurrentUser(): Observable<UserDto | null> {
    if (!this.loggedIn()) {
      this.setCurrentUser(null);
      return of(null);
    }

    const userId = this.getUserId();
    if (!userId) return of(this.currentUserSubject.value);

    return this.http.get<any>(`${this.authBaseUrl}?$filter=Id eq '${userId}'&$top=1`).pipe(
      map((res: any) => Array.isArray(res) ? res : (res?.value ?? [])),
      map((items: any[]) => items?.[0] ? this.normalizeUser(items[0]) : null),
      tap(user => this.setCurrentUser(user)),
      catchError(() => of(this.currentUserSubject.value))
    );
  }

  refreshAdminStatus(): Observable<boolean> {
    if (!this.loggedIn()) {
      this.setAdminStatus(false);
      return of(false);
    }

    const tokenAdmin = this.extractAdminFromToken();
    if (tokenAdmin) {
      this.setAdminStatus(true);
      return of(true);
    }

    const userId = this.getUserId();
    if (!userId) return of(false);

    return this.http.get<any>(`${this.userRoleBaseUrl}?$expand=Role&$filter=UserId eq '${userId}'`).pipe(
      map((res: any) => Array.isArray(res) ? res : (res?.value ?? [])),
      map((items: any[]) => items.some(x => {
        const roleName = x?.role?.name ?? x?.Role?.Name ?? x?.role?.Name ?? x?.Role?.name;
        return typeof roleName === 'string' && roleName.toLowerCase() === 'admin';
      })),
      tap(isAdmin => this.setAdminStatus(isAdmin)),
      catchError(() => of(this.adminStatusSubject.value))
    );
  }

  isAdmin(): boolean {
    return this.adminStatusSubject.value;
  }

  logout() {
    this.clearSession();
    this.router.navigate(['']);
  }

  clearSession() {
    localStorage.removeItem('token');
    localStorage.removeItem(this.cachedUserKey);
    localStorage.removeItem(this.cachedAdminKey);
    this.currentUserSubject.next(null);
    this.adminStatusSubject.next(false);
  }

  decodeToken() {
    const token = localStorage.getItem('token');
    this.decodedToken = this.jwtHelper.decodeToken(token);
    return this.decodedToken;
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  getUserId() {
    const decodedToken = this.decodeToken();
    return (
      decodedToken?.sub ??
      decodedToken?.nameid ??
      decodedToken?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
      decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/nameidentifier'] ??
      decodedToken?.userId ??
      decodedToken?.UserId
    );
  }

  getUserName() {
    const decodedToken = this.decodeToken();
    return decodedToken?.name ?? decodedToken?.unique_name ?? decodedToken?.userName ?? decodedToken?.UserName;
  }

  getFullName() {
    const decodedToken = this.decodeToken();
    return decodedToken?.FullName ?? decodedToken?.fullName;
  }

  private normalizeUser(user: any): UserDto {
    if (!user || typeof user !== 'object') return {};
    const fullName = user.fullName ?? user.FullName ?? [user.firstName ?? user.FirstName, user.lastName ?? user.LastName].filter(Boolean).join(' ');
    const firstName = user.firstName ?? user.FirstName ?? (typeof fullName === 'string' ? fullName.split(' ')[0] : undefined);
    const lastName = user.lastName ?? user.LastName ?? (typeof fullName === 'string' ? fullName.split(' ').slice(1).join(' ') : undefined);
    const normalized: UserDto = {
      id: user.id ?? user.Id,
      fullName,
      firstName,
      lastName,
      email: user.email ?? user.Email,
      userName: user.userName ?? user.UserName,
      phoneNumber: user.phoneNumber ?? user.PhoneNumber,
      imageUrl: this.normalizeUploadsUrl(user.imageUrl ?? user.ImageUrl),
      emailConfirmed: user.emailConfirmed ?? user.EmailConfirmed
    };
    return normalized;
  }

  private normalizeUploadsUrl(url: any): string | undefined {
    if (typeof url !== 'string' || !url) return undefined;
    if (/^https?:\/\//i.test(url)) return url;
    if (url.startsWith('/')) return url;
    if (url.startsWith('uploads/')) return `/${url}`;
    return url;
  }

  private setCurrentUser(user: UserDto | null) {
    this.currentUserSubject.next(user);
    if (user) {
      localStorage.setItem(this.cachedUserKey, JSON.stringify(user));
    } else {
      localStorage.removeItem(this.cachedUserKey);
    }
  }

  private readCachedUser(): UserDto | null {
    const raw = localStorage.getItem(this.cachedUserKey);
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  }

  private setAdminStatus(isAdmin: boolean) {
    this.adminStatusSubject.next(isAdmin);
    localStorage.setItem(this.cachedAdminKey, String(isAdmin));
  }

  private readCachedAdmin(): boolean {
    return localStorage.getItem(this.cachedAdminKey) === 'true';
  }

  private extractAdminFromToken(): boolean {
    const decoded = this.decodeToken();
    const candidates = [
      decoded?.role,
      decoded?.roles,
      decoded?.Role,
      decoded?.Roles,
      decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
      decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role']
    ].filter(Boolean);

    return candidates.some((claim: any) => {
      if (Array.isArray(claim)) {
        return claim.some(x => typeof x === 'string' && x.toLowerCase() === 'admin');
      }
      return typeof claim === 'string' && claim.toLowerCase() === 'admin';
    });
  }
}
