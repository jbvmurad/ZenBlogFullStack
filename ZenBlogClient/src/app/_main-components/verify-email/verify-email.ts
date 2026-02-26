import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';

declare const alertify: any;

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
        this.message = res?.Message ?? res?.message ?? 'Email verified successfully';
        if (typeof alertify !== 'undefined') {
          alertify.success(this.message);
        }
      },
      error: (err) => {
        this.status = 'error';
        this.message =
          err?.error?.Message ??
          err?.error?.message ??
          'Email verification failed. Please request a new verification email.';
        if (typeof alertify !== 'undefined') {
          alertify.error(this.message);
        }
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
