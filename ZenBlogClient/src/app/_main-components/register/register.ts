import { AfterViewInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterDto } from '../../_models/registerDto';
import { AuthService } from '../../_services/auth-service';
import { GOOGLE_CLIENT_ID } from '../../_configs/google-auth';

declare const alertify: any;
declare const google: any;

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements AfterViewInit {
  registerDto: RegisterDto = new RegisterDto();
  confirmPassword: any;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register() {
    if (this.registerDto.password !== this.confirmPassword) {
      alertify.error('Passwords do not match!');
      return;
    }

    // Server expects: { fullName, email, password }
    this.authService.register(this.registerDto).subscribe({
      next: (res) => {
        const msg = res?.Message ?? res?.message ?? 'Registration Successful! Please verify your email, then sign in.';
        alertify.success(msg);
        this.router.navigate(['/login']);
      },
      error: (err) => {
        const msg = err?.error?.Message ?? err?.error?.message ?? 'Registration Failed!';
        alertify.error(msg);
      }
    });
  }

  ngAfterViewInit(): void {
    this.renderGoogleButtonWithRetry();
  }

  private renderGoogleButtonWithRetry(triesLeft: number = 20) {
    const el = document.getElementById('googleBtnRegister');
    if (!el) return;

    if (typeof google === 'undefined' || !google?.accounts?.id) {
      if (triesLeft <= 0) return;
      setTimeout(() => this.renderGoogleButtonWithRetry(triesLeft - 1), 200);
      return;
    }

    try {
      google.accounts.id.initialize({
        client_id: GOOGLE_CLIENT_ID,
        callback: (response: any) => this.handleGoogleCredential(response)
      });

      google.accounts.id.renderButton(el, {
        theme: 'outline',
        size: 'large',
        shape: 'pill',
        text: 'signup_with',
        width: 360
      });
    } catch {
      // no-op
    }
  }

  private handleGoogleCredential(response: any) {
    const credential = response?.credential;
    if (!credential) {
      alertify.error('Google sign-up failed (no credential).');
      return;
    }

    // Server auto-registers user if not exists and returns JWT
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

        localStorage.setItem('token', token);
        alertify.success('Welcome! Signed in with Google.');
        this.router.navigate(['/admin']);
      },
      error: (err) => {
        // Try to surface the real backend/network error (Angular sometimes gives err.error as string or ProgressEvent)
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
          (typeof parsedMsg === 'string' && /untrusted\s+'aud'|audience/i.test(parsedMsg)
            ? `Google token audience uyuşmuyor. Backend'de GoogleAuth:ClientId değeri, client'daki GOOGLE_CLIENT_ID ile aynı olmalı. (Client: ${GOOGLE_CLIENT_ID})`
            : null) ??
          (err?.status === 0
            ? 'Google sign-up failed: API reachedilemedi. proxy.conf.json target adresini ve backend\'in çalıştığını kontrol et.'
            : null) ??
          `Google sign-up failed. (HTTP ${err?.status ?? 'unknown'})`;

        alertify.error(msg);
      }
    });
  }
}
