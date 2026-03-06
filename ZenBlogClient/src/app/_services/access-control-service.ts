import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, forkJoin } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { AuthService } from './auth-service';
import { RoleService, RoleDto } from './role-service';
import { UserRoleService } from './user-role-service';

@Injectable({
  providedIn: 'root'
})
export class AccessControlService {
  private readonly isAdminSubject = new BehaviorSubject<boolean>(false);
  readonly isAdmin$ = this.isAdminSubject.asObservable();

  // Cache to avoid re-fetching on every guard navigation
  private loadedForUserId: string | null = null;

  constructor(
    private auth: AuthService,
    private roleService: RoleService,
    private userRoleService: UserRoleService
  ) {
    // Best-effort warm start from localStorage
    try {
      const cached = localStorage.getItem('isAdmin');
      if (cached === 'true') this.isAdminSubject.next(true);
    } catch {
      // ignore
    }
  }

  clear() {
    this.loadedForUserId = null;
    this.isAdminSubject.next(false);
    try {
      localStorage.removeItem('isAdmin');
    } catch {
      // ignore
    }
  }

  /**
   * Loads roles for the currently logged in user (if any), then updates isAdmin$.
   * Safe to call multiple times; it will no-op if already loaded for that user.
   */
  ensureLoaded(force: boolean = false): Observable<boolean> {
    if (!this.auth.loggedIn()) {
      this.clear();
      return of(false);
    }

    const userId = this.auth.getUserId();
    if (!userId) {
      this.clear();
      return of(false);
    }

    if (!force && this.loadedForUserId === userId) {
      return of(this.isAdminSubject.value);
    }

    return forkJoin({
      roles: this.roleService.getAll().pipe(catchError(() => of([] as RoleDto[]))),
      userRoles: this.userRoleService.getForUser(userId).pipe(catchError(() => of([])))
    }).pipe(
      map(({ roles, userRoles }) => {
        const roleById = new Map<string, RoleDto>();
        roles.forEach(r => roleById.set(r.id, r));

        const roleNames = userRoles
          .map(ur => ur?.role?.name ?? roleById.get(ur.roleId)?.name)
          .filter(Boolean)
          .map(n => (n ?? '').toString().toLowerCase());

        // Treat any role that contains "admin" as admin.
        // (Some systems use Admin / Administrator / SuperAdmin, etc.)
        return roleNames.some(n => n.includes('admin'));
      }),
      tap(isAdmin => {
        this.loadedForUserId = userId;
        this.isAdminSubject.next(isAdmin);
        try {
          localStorage.setItem('isAdmin', isAdmin ? 'true' : 'false');
        } catch {
          // ignore
        }
      }),
      catchError(() => {
        // If anything goes wrong, fail closed (non-admin) but don't crash the app.
        this.loadedForUserId = userId;
        this.isAdminSubject.next(false);
        return of(false);
      })
    );
  }

  isAdminSync(): boolean {
    return this.isAdminSubject.value;
  }
}
