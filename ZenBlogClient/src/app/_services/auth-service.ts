import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  // Using a relative base URL lets Angular dev-server proxy handle CORS.
  // See: proxy.conf.json
  private authBaseUrl = '/api/Auth/';

  decodedToken: any;
  jwtHelper = new JwtHelperService();

  login(model: any) {
    return this.http.post<any>(this.authBaseUrl + 'login', model);
  }

  loginWithGoogle(idToken: string) {
    return this.http.post<any>(this.authBaseUrl + 'login-google', { idToken });
  }

  register(model: any) {
    return this.http.post<any>(this.authBaseUrl + 'register', model);
  }

  confirmEmail(userId: string, token: string) {
    return this.http.post<any>(this.authBaseUrl + 'confirm-email', { userId, token });
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['']);
  }

  decodeToken() {
    const token = localStorage.getItem('token');
    this.decodedToken = this.jwtHelper.decodeToken(token);
    return this.decodedToken;
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  getUserId() {
    const decodedToken = this.decodeToken();
    // Server puts user id in ClaimTypes.NameIdentifier
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
}
