import { AfterViewInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterDto } from '../../_models/registerDto';
import { AuthService } from '../../_services/auth-service';
import { GoogleIdentityService } from '../../_services/google-identity-service';

declare const alertify: any;

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements AfterViewInit {
  registerDto: RegisterDto = new RegisterDto();

  constructor(
    private authService: AuthService,
    private router: Router,
    private googleIdentityService: GoogleIdentityService
  ) {}

  register() {
    const payload = {
      fullName: this.registerDto.fullName,
      email: this.registerDto.email,
      password: this.registerDto.password,
      confirmPassword: this.registerDto.confirmPassword
    };

    this.authService.register(payload).subscribe({
      next: (res) => {
        const msg =
          res?.Message ??
          res?.message ??
          'Registration Successful! Please verify your email, then sign in.';
        alertify.success(msg);
        this.router.navigate(['/login']);
      },
      error: (err) => {
        const msg =
          err?.error?.Message ??
          err?.error?.message ??
          (err?.error?.errors
            ? Object.values(err.error.errors).flat().join(' ')
            : null) ??
          'Registration Failed!';
        alertify.error(msg);
      }
    });
  }

  ngAfterViewInit(): void {
    this.googleIdentityService
      .renderButton('googleBtnRegister', 'signup_with', (response: any) => this.handleGoogleCredential(response))
      .catch(() => {
        alertify.error('Google sign-up button could not be loaded.');
      });
  }

  private handleGoogleCredential(response: any) {
    const credential = response?.credential;
    if (!credential) {
      alertify.error('Google sign-up failed (no credential).');
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
          alertify.error('Google sign-up failed (Token not found in response).');
          return;
        }

        this.authService.setSessionToken(token).subscribe({
          next: () => {
            alertify.success('Welcome! Signed in with Google.');
            this.router.navigate(['/admin']);
          },
          error: () => {
            alertify.success('Welcome! Signed in with Google.');
            this.router.navigate(['/admin']);
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
            ? "Google sign-up failed: API reachedilemedi. proxy.conf.json target adresini ve backend'in çalıştığını kontrol et."
            : null) ??
          `Google sign-up failed. (HTTP ${err?.status ?? 'unknown'})`;

        alertify.error(msg);
      }
    });
  }
}
