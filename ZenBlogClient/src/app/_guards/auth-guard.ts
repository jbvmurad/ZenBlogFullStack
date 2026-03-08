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
    if (!adminOnly) return true;

    return this.authService.refreshAdminStatus().pipe(
      map(isAdmin => {
        if (isAdmin) return true;
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
