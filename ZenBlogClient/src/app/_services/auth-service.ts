import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, Observable, catchError, forkJoin, map, of, tap } from 'rxjs';
import { UserDto } from '../_models/userDto';

interface RoleAccessState {
  isAdmin: boolean;
  isManager: boolean;
  hasDashboardAccess: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authBaseUrl = '/api/Auth';
  private userRoleBaseUrl = '/api/UserRole';
  private cachedUserKey = 'currentUserProfile';
  private cachedAdminKey = 'currentUserIsAdmin';
  private cachedManagerKey = 'currentUserIsManager';
  private cachedDashboardAccessKey = 'currentUserHasDashboardAccess';
  private accountMenuVisibleKey = 'accountMenuVisible';
  private refreshTokenKey = 'refreshToken';
  private refreshTokenExpiresKey = 'refreshTokenExpires';

  decodedToken: any;
  jwtHelper = new JwtHelperService();

  private currentUserSubject = new BehaviorSubject<UserDto | null>(this.readCachedUser());
  currentUser$ = this.currentUserSubject.asObservable();

  private adminStatusSubject = new BehaviorSubject<boolean>(this.readCachedAdmin());
  isAdmin$ = this.adminStatusSubject.asObservable();

  private managerStatusSubject = new BehaviorSubject<boolean>(this.readCachedBoolean(this.cachedManagerKey));
  isManager$ = this.managerStatusSubject.asObservable();

  private dashboardAccessSubject = new BehaviorSubject<boolean>(this.readCachedBoolean(this.cachedDashboardAccessKey));
  hasDashboardAccess$ = this.dashboardAccessSubject.asObservable();

  private accountMenuVisibleSubject = new BehaviorSubject<boolean>(this.readAccountMenuVisible());
  accountMenuVisible$ = this.accountMenuVisibleSubject.asObservable();

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

  setSessionToken(authResult: string | any): Observable<void> {
    const token = this.extractToken(authResult);
    const refreshToken = this.extractRefreshToken(authResult);
    const refreshTokenExpires = this.extractRefreshTokenExpires(authResult);

    if (!token) {
      return of(void 0);
    }

    localStorage.setItem('token', token);

    if (refreshToken) {
      localStorage.setItem(this.refreshTokenKey, refreshToken);
    } else {
      localStorage.removeItem(this.refreshTokenKey);
    }

    if (refreshTokenExpires) {
      localStorage.setItem(this.refreshTokenExpiresKey, refreshTokenExpires);
    } else {
      localStorage.removeItem(this.refreshTokenExpiresKey);
    }

    this.setAccountMenuVisible(false);
    this.decodeToken();

    return forkJoin({
      user: this.refreshCurrentUser().pipe(catchError(() => of(null))),
      roles: this.refreshRoleAccess().pipe(catchError(() => of(this.emptyRoleAccess())))
    }).pipe(
      map(() => void 0)
    );
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
    removeImage?: boolean;
  }, imageFile?: File | null) {
    const form = this.buildUserUpdateFormData(model, imageFile);

    return this.http.put<any>(this.authBaseUrl, form).pipe(
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
    removeImage?: boolean;
  }, imageFile?: File | null) {
    return this.update(model, imageFile);
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
    return this.refreshRoleAccess().pipe(
      map(access => access.isAdmin)
    );
  }

  refreshDashboardAccessStatus(): Observable<boolean> {
    return this.refreshRoleAccess().pipe(
      map(access => access.hasDashboardAccess)
    );
  }

  refreshRoleAccess(): Observable<RoleAccessState> {
    if (!this.loggedIn()) {
      const emptyState = this.emptyRoleAccess();
      this.setRoleAccess(emptyState);
      return of(emptyState);
    }

    const tokenAccess = this.mapRoleAccess(this.extractRoleNamesFromToken());

    const userId = this.getUserId();
    if (!userId || !tokenAccess.hasDashboardAccess) {
      this.setRoleAccess(tokenAccess);
      return of(tokenAccess);
    }

    return this.http.get<any>(`${this.userRoleBaseUrl}?$filter=UserId eq '${userId}'`).pipe(
      map((res: any) => Array.isArray(res) ? res : (res?.value ?? [])),
      map((items: any[]) => items
        .map(x => x?.roleName ?? x?.RoleName ?? x?.role?.name ?? x?.Role?.Name ?? x?.role?.Name ?? x?.Role?.name)
        .filter((roleName: unknown): roleName is string => typeof roleName === 'string' && !!roleName.trim())),
      map(roleNames => this.mapRoleAccess(roleNames)),
      tap(access => this.setRoleAccess(access)),
      catchError((error: HttpErrorResponse) => {
        if (error?.status === 401 || error?.status === 403) {
          this.setRoleAccess(tokenAccess);
          return of(tokenAccess);
        }

        const current = this.currentRoleAccess();
        this.setRoleAccess(current);
        return of(current);
      })
    );
  }

  isAdmin(): boolean {
    return this.adminStatusSubject.value;
  }

  isManager(): boolean {
    return this.managerStatusSubject.value;
  }

  hasDashboardAccess(): boolean {
    return this.dashboardAccessSubject.value;
  }

  getPostLoginRoute(): string {
    return this.currentUserSubject.value?.isProtectedDashboardAdmin ? '/admin/category' : '/';
  }

  logout() {
    this.completeLogout();
  }

  showAuthenticatedUi() {
    if (!this.loggedIn()) {
      this.setAccountMenuVisible(false);
      return;
    }

    this.setAccountMenuVisible(true);
  }

  shouldShowAuthenticatedUi(): boolean {
    return this.loggedIn() && this.accountMenuVisibleSubject.value;
  }

  clearSession() {
    localStorage.removeItem('token');
    localStorage.removeItem(this.cachedUserKey);
    localStorage.removeItem(this.cachedAdminKey);
    localStorage.removeItem(this.cachedManagerKey);
    localStorage.removeItem(this.cachedDashboardAccessKey);
    localStorage.removeItem(this.accountMenuVisibleKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.refreshTokenExpiresKey);
    this.currentUserSubject.next(null);
    this.adminStatusSubject.next(false);
    this.managerStatusSubject.next(false);
    this.dashboardAccessSubject.next(false);
    this.accountMenuVisibleSubject.next(false);
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

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  private buildUserUpdateFormData(model: {
    id: string;
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    password?: string;
    imageUrl?: string;
    removeImage?: boolean;
  }, imageFile?: File | null): FormData {
    const form = new FormData();
    form.append('Id', model.id ?? '');

    if (model.fullName != null) form.append('FullName', model.fullName);
    if (model.email != null) form.append('Email', model.email);
    if (model.phoneNumber != null) form.append('PhoneNumber', model.phoneNumber);
    if (model.password != null) form.append('Password', model.password);
    if (typeof model.removeImage === 'boolean') form.append('RemoveImage', String(model.removeImage));
    if (imageFile) form.append('Image', imageFile);

    return form;
  }

  private completeLogout() {
    this.clearSession();
    this.router.navigate(['']);
  }

  private setAccountMenuVisible(value: boolean) {
    localStorage.setItem(this.accountMenuVisibleKey, value ? 'true' : 'false');
    this.accountMenuVisibleSubject.next(value);
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
      emailConfirmed: user.emailConfirmed ?? user.EmailConfirmed,
      isProtectedDashboardAdmin: user.isProtectedDashboardAdmin ?? user.IsProtectedDashboardAdmin
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
    if (user) {
      localStorage.setItem(this.cachedUserKey, JSON.stringify(user));
    } else {
      localStorage.removeItem(this.cachedUserKey);
    }

    this.currentUserSubject.next(user);
  }

  private setAdminStatus(isAdmin: boolean) {
    localStorage.setItem(this.cachedAdminKey, isAdmin ? 'true' : 'false');
    this.adminStatusSubject.next(isAdmin);
  }

  private setManagerStatus(isManager: boolean) {
    localStorage.setItem(this.cachedManagerKey, isManager ? 'true' : 'false');
    this.managerStatusSubject.next(isManager);
  }

  private setDashboardAccessStatus(hasDashboardAccess: boolean) {
    localStorage.setItem(this.cachedDashboardAccessKey, hasDashboardAccess ? 'true' : 'false');
    this.dashboardAccessSubject.next(hasDashboardAccess);
  }

  private setRoleAccess(access: RoleAccessState) {
    this.setAdminStatus(access.isAdmin);
    this.setManagerStatus(access.isManager);
    this.setDashboardAccessStatus(access.hasDashboardAccess);
  }

  private readCachedUser(): UserDto | null {
    const raw = localStorage.getItem(this.cachedUserKey);
    if (!raw) return null;

    try {
      return JSON.parse(raw) as UserDto;
    } catch {
      localStorage.removeItem(this.cachedUserKey);
      return null;
    }
  }

  private readCachedAdmin(): boolean {
    return localStorage.getItem(this.cachedAdminKey) === 'true';
  }

  private readCachedBoolean(key: string): boolean {
    return localStorage.getItem(key) === 'true';
  }

  private readAccountMenuVisible(): boolean {
    return localStorage.getItem(this.accountMenuVisibleKey) === 'true';
  }

  private extractToken(authResult: any): string | null {
    if (typeof authResult === 'string') return authResult;

    return (
      authResult?.Token ??
      authResult?.token ??
      authResult?.data?.Token ??
      authResult?.data?.token ??
      null
    );
  }

  private extractRefreshToken(authResult: any): string | null {
    if (typeof authResult === 'string') return null;

    return (
      authResult?.RefreshToken ??
      authResult?.refreshToken ??
      authResult?.data?.RefreshToken ??
      authResult?.data?.refreshToken ??
      null
    );
  }

  private extractRefreshTokenExpires(authResult: any): string | null {
    if (typeof authResult === 'string') return null;

    const value =
      authResult?.RefreshTokenExpires ??
      authResult?.refreshTokenExpires ??
      authResult?.data?.RefreshTokenExpires ??
      authResult?.data?.refreshTokenExpires ??
      null;

    return value == null ? null : String(value);
  }

  private extractRoleNamesFromToken(): string[] {
    const decodedToken = this.decodeToken();
    if (!decodedToken) return [];

    const roleClaim =
      decodedToken?.role ??
      decodedToken?.roles ??
      decodedToken?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      decodedToken?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'];

    if (Array.isArray(roleClaim)) {
      return roleClaim.filter((x: any) => typeof x === 'string' && !!x.trim());
    }

    return typeof roleClaim === 'string' && roleClaim.trim() ? [roleClaim] : [];
  }

  private mapRoleAccess(roleNames: string[]): RoleAccessState {
    const normalizedRoles = roleNames.map(role => role.trim().toLowerCase());
    const isAdmin = normalizedRoles.includes('admin');
    const isManager = normalizedRoles.includes('manager');

    return {
      isAdmin,
      isManager,
      hasDashboardAccess: isAdmin || isManager
    };
  }

  private currentRoleAccess(): RoleAccessState {
    return {
      isAdmin: this.adminStatusSubject.value,
      isManager: this.managerStatusSubject.value,
      hasDashboardAccess: this.dashboardAccessSubject.value
    };
  }

  private emptyRoleAccess(): RoleAccessState {
    return {
      isAdmin: false,
      isManager: false,
      hasDashboardAccess: false
    };
  }
}
