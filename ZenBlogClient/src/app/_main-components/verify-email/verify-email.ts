import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';

@Component({
  selector: 'app-verify-email',
  standalone: false,
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.css'
})
export class VerifyEmail implements OnInit {
  status: 'loading' | 'success' | 'error' = 'loading';
  message = 'Verifying your email...';

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (!userId || !token) {
      this.status = 'error';
      this.message = 'Verification link is invalid or incomplete.';
      return;
    }

    this.authService.confirmEmail(userId, token).subscribe({
      next: (res) => {
        this.status = 'success';
        // Show the server message directly on this page (no popups, no redirect)
        this.message =
          res?.Message ??
          res?.message ??
          res?.Error ??
          res?.error ??
          'Email verified successfully';
      },
      error: (err) => {
        this.status = 'error';
        // Try multiple shapes because backend middleware may return {message} or {error}.
        // Also handle FluentValidation style {errors: {field: [..]}}.
        const e = err?.error;
        const firstValidation = e?.errors ? (Object.values(e.errors).flat() as any)?.[0] : null;

        this.message =
          e?.Message ??
          e?.message ??
          e?.Error ??
          e?.error ??
          firstValidation ??
          'Email verification failed. Please request a new verification email.';
      }
    });
  }

  goLogin() {
    this.router.navigate(['/login']);
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
