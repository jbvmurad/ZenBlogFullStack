import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth-service';

declare const alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {


  constructor(private authserService: AuthService,
              private router: Router
  ){}


  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {

if(this.authserService.loggedIn()){
  return true;
}
console.log("auth guard");
try { alertify?.error?.('Please login first.'); } catch {}
this.router.navigate(["/login"], { queryParams: { returnUrl: state.url } });
return false;

  }


}
