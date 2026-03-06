import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  GuardResult,
  MaybeAsync,
  Router,
  RouterStateSnapshot
} from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AuthService } from '../_services/auth-service';
import { AccessControlService } from '../_services/access-control-service';
import { RoleService, RoleDto } from '../_services/role-service';
import { UserRoleService } from '../_services/user-role-service';

declare const alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private auth: AuthService,
    private access: AccessControlService,
    private roleService: RoleService,
    private userRoleService: UserRoleService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
    if (!this.auth.loggedIn()) {
      try { alertify?.error?.('Please login first.'); } catch {}
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return false;
    }

    const userId = this.auth.getUserId();

    // 1) Normal case: only Admin users can access /admin
    // 2) Bootstrap case: if there is *no* admin assigned yet, allow access only to /admin/users
    return forkJoin({
      roles: this.roleService.getAll().pipe(catchError(() => of([] as RoleDto[]))),
      allUserRoles: this.userRoleService.getAll().pipe(catchError(() => of([]))),
      myUserRoles: userId ? this.userRoleService.getForUser(userId).pipe(catchError(() => of([]))) : of([])
    }).pipe(
      map(({ roles, allUserRoles, myUserRoles }) => {
        const roleById = new Map<string, RoleDto>();
        roles.forEach(r => roleById.set(r.id, r));

        const isAdminRoleName = (name: any) => (name ?? '').toString().toLowerCase().includes('admin');
        const roleNameOf = (ur: any) => ur?.role?.name ?? roleById.get(ur?.roleId)?.name;

        const meIsAdmin = myUserRoles.some((ur: any) => isAdminRoleName(roleNameOf(ur)));
        if (meIsAdmin) {
          this.access.ensureLoaded(true).subscribe();
          return true;
        }

        const anyAdminAssigned = allUserRoles.some((ur: any) => isAdminRoleName(roleNameOf(ur)));
        const isBootstrapUsersPage = (state.url ?? '').startsWith('/admin/users');

        if (!anyAdminAssigned && isBootstrapUsersPage) {
          // Allow a first-time setup to create/assign the Admin role.
          try { alertify?.warning?.('No admin is assigned yet. Configure admin access on this page.'); } catch {}
          return true;
        }

        try { alertify?.error?.('You are not authorized to access the admin dashboard.'); } catch {}
        return this.router.parseUrl('');
      })
    );
  }
}
