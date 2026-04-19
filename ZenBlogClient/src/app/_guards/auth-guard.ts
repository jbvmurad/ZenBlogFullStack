import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth-service';
import { catchError, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ){}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
    if (!this.authService.loggedIn()) {
      this.router.navigate(['']);
      return false;
    }

    const adminOnly = route.data?.['adminOnly'] === true;
    const dashboardOnly = route.data?.['dashboardOnly'] === true;

    if (!adminOnly && !dashboardOnly) return true;

    const accessCheck = adminOnly
      ? this.authService.refreshAdminStatus()
      : this.authService.refreshDashboardAccessStatus();

    return accessCheck.pipe(
      map(isAllowed => {
        if (isAllowed) return true;
        this.router.navigate(['']);
        return false;
      }),
      catchError(() => {
        this.router.navigate(['']);
        return of(false);
      })
    );
  }
}
