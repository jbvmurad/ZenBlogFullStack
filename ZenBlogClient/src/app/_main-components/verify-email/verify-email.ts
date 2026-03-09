import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';

@Component({
  selector: 'app-verify-email',
  standalone: false,
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.css'
})
export class VerifyEmail implements OnInit, OnDestroy {
  status: 'loading' | 'success' | 'error' = 'loading';
  message = 'Verifying your email...';
  redirectCountdown = 5;
  private redirectTimer: ReturnType<typeof setInterval> | null = null;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const qp = this.route.snapshot.queryParamMap;
    const fragmentParams = this.readFragmentParams(this.route.snapshot.fragment);

    const userId =
      qp.get('userId') ??
      qp.get('id') ??
      qp.get('uid') ??
      fragmentParams.get('userId') ??
      fragmentParams.get('id') ??
      fragmentParams.get('uid');

    let token =
      qp.get('token') ??
      qp.get('code') ??
      qp.get('emailToken') ??
      fragmentParams.get('token') ??
      fragmentParams.get('code') ??
      fragmentParams.get('emailToken');

    token = token?.replace(/ /g, '+') ?? null;

    if (!userId || !token) {
      this.status = 'error';
      this.message = 'Verification link is invalid or incomplete.';
      return;
    }

    this.authService.confirmEmail(userId, token).subscribe({
      next: (res) => {
        this.status = 'success';
        this.message =
          res?.Message ??
          res?.message ??
          res?.Error ??
          res?.error ??
          'Email verified successfully';
        this.startRedirectCountdown();
      },
      error: (err) => {
        this.status = 'error';
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

  ngOnDestroy(): void {
    this.clearRedirectTimer();
  }

  goLogin() {
    this.clearRedirectTimer();
    this.router.navigate(['/login']);
  }

  goHome() {
    this.clearRedirectTimer();
    this.router.navigate(['/']);
  }

  private startRedirectCountdown() {
    this.clearRedirectTimer();
    this.redirectCountdown = 5;
    this.redirectTimer = setInterval(() => {
      this.redirectCountdown -= 1;
      if (this.redirectCountdown <= 0) {
        this.goLogin();
      }
    }, 1000);
  }

  private clearRedirectTimer() {
    if (this.redirectTimer) {
      clearInterval(this.redirectTimer);
      this.redirectTimer = null;
    }
  }

  private readFragmentParams(fragment: string | null): URLSearchParams {
    if (!fragment) return new URLSearchParams();
    const queryLike = fragment.includes('?') ? fragment.split('?')[1] : fragment;
    return new URLSearchParams(queryLike);
  }
}
