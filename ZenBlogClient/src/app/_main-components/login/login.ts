import { AfterViewInit, Component } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { LoginDto } from '../../_models/loginDto';
import { Router } from '@angular/router';
import { GoogleIdentityService } from '../../_services/google-identity-service';

declare const alertify: any;

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements AfterViewInit {
  constructor(
    private authService: AuthService,
    private router: Router,
    private googleIdentityService: GoogleIdentityService
  ) {}

  loginDto: LoginDto = new LoginDto();
  token: any;
  decodedToken: any;

  login() {
    this.authService.login(this.loginDto).subscribe({
      next: result => {
        const token =
          result?.Token ??
          result?.token ??
          result?.data?.Token ??
          result?.data?.token;

        this.token = token;

        if (!token) {
          alertify.error('Login Failed! (Token not found in response)');
          return;
        }

        this.authService.setSessionToken(token).subscribe({
          next: () => {
            alertify.success('Login Successful!');
            this.authService.showAuthenticatedUi();
            this.router.navigate(['/']);
          },
          error: () => {
            alertify.success('Login Successful!');
            this.authService.showAuthenticatedUi();
            this.router.navigate(['/']);
          }
        });
      },
      error: result => {
        const msg =
          result?.error?.Message ??
          result?.error?.message ??
          (result?.error?.errors ? JSON.stringify(result.error.errors) : null) ??
          'Login Failed!';

        alertify.error(msg);
      }
    });
  }

  ngAfterViewInit(): void {
    this.googleIdentityService
      .renderButton('googleBtn', 'signin_with', (response: any) => this.handleGoogleCredential(response))
      .catch(() => {
        alertify.error('Google sign-in button could not be loaded.');
      });
  }

  private handleGoogleCredential(response: any) {
    const credential = response?.credential;
    if (!credential) {
      alertify.error('Google sign-in failed (no credential).');
      return;
    }

    this.authService.loginWithGoogle(credential).subscribe({
      next: (result) => {
        const token =
          result?.Token ??
          result?.token ??
          result?.data?.Token ??
          result?.data?.token;

        if (!token) {
          alertify.error('Google sign-in failed (Token not found in response).');
          return;
        }

        this.authService.setSessionToken(token).subscribe({
          next: () => {
            alertify.success('Signed in with Google!');
            this.authService.showAuthenticatedUi();
            this.router.navigate(['/']);
          },
          error: () => {
            alertify.success('Signed in with Google!');
            this.authService.showAuthenticatedUi();
            this.router.navigate(['/']);
          }
        });
      },
      error: (err) => {
        let parsedMsg: string | null = null;

        if (typeof err?.error === 'string') {
          try {
            const obj = JSON.parse(err.error);
            parsedMsg = obj?.Message ?? obj?.message ?? null;
          } catch {
            parsedMsg = err.error;
          }
        }

        const msg =
          err?.error?.Message ??
          err?.error?.message ??
          parsedMsg ??
          (err?.status === 0
            ? 'Google sign-in failed: API reachedilemedi. proxy.conf.json target adresini ve backend\'in çalıştığını kontrol et.'
            : null) ??
          `Google sign-in failed. (HTTP ${err?.status ?? 'unknown'})`;

        alertify.error(msg);
      }
    });
  }

  decodeToken() {
    const decodedToken = this.authService.decodeToken();
    this.decodedToken = decodedToken;
  }
}