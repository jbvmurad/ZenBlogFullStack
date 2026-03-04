import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';

declare const alertify: any;

@Component({
  selector: 'app-forgot-password',
  standalone: false,
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {
  email: any;
  isSubmitting = false;

  constructor(private authService: AuthService, private router: Router) {}

  submit() {
    this.isSubmitting = true;
    const payload = { email: this.email };

    this.authService.forgotPassword(payload).subscribe({
      next: (res) => {
        const msg = res?.Message ?? res?.message ?? 'If that email exists, we sent a reset link.';
        alertify.success(msg);
        this.isSubmitting = false;
        // keep user on page; they can go login
      },
      error: (err) => {
        const msg =
          err?.error?.Message ??
          err?.error?.message ??
          (err?.error?.errors ? Object.values(err.error.errors).flat().join(' ') : null) ??
          'Request failed.';
        alertify.error(msg);
        this.isSubmitting = false;
      }
    });
  }

  goLogin() {
    this.router.navigate(['/login']);
  }
}
