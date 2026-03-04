import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';

declare const alertify: any;

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword implements OnInit {
  userId: string | null = null;
  token: string | null = null;

  newPassword: any;
  confirmPassword: any;
  isSubmitting = false;

  constructor(private route: ActivatedRoute, private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.userId = this.route.snapshot.queryParamMap.get('userId');
    this.token = this.route.snapshot.queryParamMap.get('token');

    if (!this.userId || !this.token) {
      alertify.error('Reset link is invalid or incomplete.');
    }
  }

  submit() {
    if (!this.userId || !this.token) {
      alertify.error('Reset link is invalid or incomplete.');
      return;
    }

    // Client-side check only for confirm; backend doesn't have confirmPassword field.
    if (this.newPassword !== this.confirmPassword) {
      alertify.error('Passwords do not match!');
      return;
    }

    this.isSubmitting = true;

    const payload = {
      userId: this.userId,
      token: this.token,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword
    };

    this.authService.resetPassword(payload).subscribe({
      next: (res) => {
        const msg = res?.Message ?? res?.message ?? 'Password updated successfully.';
        alertify.success(msg);
        this.isSubmitting = false;
        this.router.navigate(['/login']);
      },
      error: (err) => {
        const msg =
          err?.error?.Message ??
          err?.error?.message ??
          (err?.error?.errors ? Object.values(err.error.errors).flat().join(' ') : null) ??
          'Reset failed.';
        alertify.error(msg);
        this.isSubmitting = false;
      }
    });
  }

  goLogin() {
    this.router.navigate(['/login']);
  }
}
