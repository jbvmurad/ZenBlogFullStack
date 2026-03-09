import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../_services/auth-service';
declare const alertify: any;
declare const Swal: any;

@Component({
  selector: 'app-settings',
  standalone: false,
  templateUrl: './settings.html',
  styleUrl: './settings.css'
})
export class Settings implements OnInit {
  form: any = {
    id: '',
    fullName: '',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
    imageUrl: ''
  };

  selectedImage: File | null = null;
  previewUrl: string | null = null;
  saving = false;
  deleting = false;
  enablePasswordChange = false;
  private authEmailForConfirmation = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.getCurrentUser(true).subscribe(user => {
      this.form.id = user?.id ?? this.authService.getUserId() ?? '';
      this.form.fullName = user?.fullName ?? this.authService.getFullName() ?? '';
      this.form.email = user?.email ?? '';
      this.form.phoneNumber = user?.phoneNumber ?? '';
      this.form.imageUrl = user?.imageUrl ?? '';
      this.previewUrl = user?.imageUrl ?? null;
      this.authEmailForConfirmation = user?.email ?? this.authService.getUserName() ?? '';
    });
  }

  onImageSelected(event: any) {
    const file = event?.target?.files?.[0] as File | undefined;
    if (!file) return;
    this.selectedImage = file;
    this.previewUrl = URL.createObjectURL(file);
  }

  clearSelectedImage() {
    this.selectedImage = null;
    this.previewUrl = this.form.imageUrl || null;
  }

  onPasswordToggleChange() {
    if (!this.enablePasswordChange) {
      this.form.password = '';
    }
  }

  save() {
    if (!this.form.id) {
      alertify.error('User id not found.');
      return;
    }

    if (!this.form.fullName || !this.form.email) {
      alertify.error('Full name and email are required.');
      return;
    }

    if (!this.form.confirmPassword?.trim()) {
      alertify.error('Please enter your confirm password first.');
      return;
    }

    if (this.enablePasswordChange && !this.form.password?.trim()) {
      alertify.error('Please enter your new password.');
      return;
    }

    const authEmail = this.authEmailForConfirmation || this.form.email;
    if (!authEmail) {
      alertify.error('Confirmation email could not be determined.');
      return;
    }

    this.saving = true;

    this.authService.login({
      email: authEmail,
      password: this.form.confirmPassword
    }).subscribe({
      next: () => this.performUpdate(),
      error: () => {
        this.saving = false;
        alertify.error('Confirm password is incorrect. Changes were not saved.');
      }
    });
  }

  private performUpdate() {
    const request = {
      id: this.form.id,
      fullName: this.form.fullName,
      email: this.form.email,
      phoneNumber: this.form.phoneNumber,
      password: this.enablePasswordChange ? this.form.password : '',
      imageUrl: this.form.imageUrl
    };

    const action = this.selectedImage
      ? this.authService.updateWithMedia(request, this.selectedImage)
      : this.authService.update(request);

    action.subscribe({
      next: () => {
        alertify.success('Profile updated successfully.');
        this.selectedImage = null;
        this.form.password = '';
        this.form.confirmPassword = '';
        this.enablePasswordChange = false;
        this.authEmailForConfirmation = this.form.email;
        this.authService.refreshCurrentUser().subscribe(() => {
          this.saving = false;
          this.router.navigate(['/profile']);
        });
      },
      error: (err) => {
        this.saving = false;
        const msg = err?.error?.Message ?? err?.error?.message ?? 'Profile update failed.';
        alertify.error(msg);
      }
    });
  }

  deleteAccount() {
    const userId = this.form.id;
    if (!userId) return;

    Swal.fire({
      title: 'Delete account?',
      text: 'This action cannot be undone.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it',
      cancelButtonText: 'Cancel'
    }).then((result: any) => {
      if (!result?.isConfirmed) return;

      this.deleting = true;
      this.authService.deleteUser(userId).subscribe({
        next: () => {
          this.deleting = false;
          alertify.success('Your account has been deleted.');
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.deleting = false;
          const msg = err?.error?.Message ?? err?.error?.message ?? 'Account delete failed.';
          alertify.error(msg);
        }
      });
    });
  }

  get initials(): string {
    const value = this.form.fullName || this.form.email || 'U';
    return value
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((x: string) => x[0])
      .join('')
      .toUpperCase();
  }
}
