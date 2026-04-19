import { AfterViewInit, Component } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterDto } from '../../_models/registerDto';
import { AuthService } from '../../_services/auth-service';
import { GoogleIdentityService } from '../../_services/google-identity-service';

declare const alertify: any;

interface GoogleRegisterProfile {
  fullName: string;
  email: string;
}

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements AfterViewInit {
  registerDto: RegisterDto = new RegisterDto();
  googleProfile: GoogleRegisterProfile | null = null;

  constructor(
    private authService: AuthService,
    private router: Router,
    private googleIdentityService: GoogleIdentityService
  ) {}

  get isGoogleCompletionStep(): boolean {
    return this.googleProfile !== null;
  }

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
        this.googleProfile = null;
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

  clearGoogleSelection() {
    this.googleProfile = null;
    this.registerDto.fullName = null;
    this.registerDto.email = null;
    this.registerDto.password = null;
    this.registerDto.confirmPassword = null;
  }

  private handleGoogleCredential(response: any) {
    const credential = response?.credential;
    if (!credential) {
      alertify.error('Google sign-up failed (no credential).');
      return;
    }

    const profile = this.decodeGoogleCredential(credential);
    if (!profile?.email) {
      alertify.error('Google sign-up failed: account information could not be read.');
      return;
    }

    this.googleProfile = profile;
    this.registerDto.fullName = profile.fullName;
    this.registerDto.email = profile.email;
    this.registerDto.password = null;
    this.registerDto.confirmPassword = null;

    alertify.success('Google account selected. Set your password to complete registration.');
  }

  private decodeGoogleCredential(credential: string): GoogleRegisterProfile | null {
    try {
      const parts = credential.split('.');
      if (parts.length < 2) return null;

      const payload = JSON.parse(this.base64UrlDecode(parts[1]));
      return {
        fullName: payload?.name ?? payload?.given_name ?? '',
        email: payload?.email ?? ''
      };
    } catch {
      return null;
    }
  }

  private base64UrlDecode(value: string): string {
    const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
    const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '=');
    return decodeURIComponent(
      atob(padded)
        .split('')
        .map((char) => `%${(`00${char.charCodeAt(0).toString(16)}`).slice(-2)}`)
        .join('')
    );
  }
}
