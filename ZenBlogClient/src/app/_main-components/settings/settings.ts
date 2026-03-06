import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';
import { UserService, UserDto } from '../../_services/user-service';
import { AccessControlService } from '../../_services/access-control-service';
import Swal from 'sweetalert2';

declare const alertify: any;

@Component({
  selector: 'app-settings',
  standalone: false,
  templateUrl: './settings.html',
  styleUrl: './settings.css'
})
export class Settings {
  loading = true;
  saving = false;
  user: UserDto | null = null;

  // UI toggle: user can update profile info without changing password,
  // but backend still requires a password value on update.
  changePassword = false;

  form: any;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private userService: UserService,
    private access: AccessControlService,
    private router: Router
  ) {
    this.form = this.fb.group({
      fullName: [''],
      email: ['', [Validators.email]],
      phoneNumber: [''],
      currentPassword: ['', [Validators.required]],
      newPassword: [''],
      confirmNewPassword: ['']
    });
    this.load();
  }

  private load() {
    this.loading = true;
    const userId = this.auth.getUserId();
    if (!userId) {
      this.loading = false;
      try { alertify?.error?.('User not found in token.'); } catch {}
      this.router.navigate(['']);
      return;
    }

    this.userService.getById(userId).subscribe({
      next: (u) => {
        this.user = u;
        this.form.patchValue({
          fullName: u?.fullName ?? '',
          email: u?.email ?? '',
          phoneNumber: u?.phoneNumber ?? ''
        });
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.user = null;
        this.loading = false;
        try { alertify?.error?.('Failed to load settings.'); } catch {}
      }
    });
  }

  save() {
    if (!this.user?.id) return;
    if (this.form.invalid) {
      try { alertify?.error?.('Please check the form fields.'); } catch {}
      return;
    }

    const currentPassword = (this.form.value.currentPassword ?? '').trim();
    if (!currentPassword) {
      try { alertify?.error?.('Current password is required to save changes.'); } catch {}
      return;
    }

    const newPassword = (this.form.value.newPassword ?? '').trim();
    const confirmNewPassword = (this.form.value.confirmNewPassword ?? '').trim();

    if (this.changePassword) {
      if (!newPassword || !confirmNewPassword) {
        try { alertify?.error?.('Please enter and confirm your new password.'); } catch {}
        return;
      }
      if (newPassword !== confirmNewPassword) {
        try { alertify?.error?.('New passwords do not match.'); } catch {}
        return;
      }
    }

    this.saving = true;

    const passwordToSend = this.changePassword ? newPassword : currentPassword;
    const payload = {
      Id: this.user.id,
      FullName: (this.form.value.fullName ?? '').trim() || null,
      Email: (this.form.value.email ?? '').trim() || null,
      PhoneNumber: (this.form.value.phoneNumber ?? '').trim() || null,
      Password: passwordToSend || null
    };

    this.userService.update(payload).subscribe({
      next: (res: any) => {
        this.saving = false;
        try { alertify?.success?.(res?.message ?? 'Profile updated.'); } catch {}
        // Password fields should be cleared after save
        this.changePassword = false;
        this.form.patchValue({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
        this.load();
      },
      error: (err) => {
        console.error(err);
        this.saving = false;
        const msg = err?.error?.message ?? err?.error?.Message ?? err?.message ?? 'Update failed.';
        try { alertify?.error?.(msg); } catch {}
      }
    });
  }

  async deleteAccount() {
    const userId = this.user?.id;
    if (!userId) return;

    const result = await Swal.fire({
      title: 'Delete account?',
      text: 'This action is permanent and cannot be undone.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#d33'
    });

    if (!result.isConfirmed) return;

    this.userService.delete(userId).subscribe({
      next: (res: any) => {
        try { alertify?.success?.(res?.message ?? 'Account deleted.'); } catch {}
        this.access.clear();
        localStorage.removeItem('token');
        this.router.navigate(['']);
      },
      error: (err) => {
        console.error(err);
        const msg = err?.error?.message ?? err?.error?.Message ?? err?.message ?? 'Delete failed.';
        try { alertify?.error?.(msg); } catch {}
      }
    });
  }
}
