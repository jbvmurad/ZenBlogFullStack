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
    imageUrl: ''
  };

  selectedImage: File | null = null;
  previewUrl: string | null = null;
  saving = false;
  deleting = false;

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

  save() {
    if (!this.form.id) {
      alertify.error('User id not found.');
      return;
    }

    if (!this.form.fullName || !this.form.email) {
      alertify.error('Full name and email are required.');
      return;
    }

    this.saving = true;

    const request = {
      id: this.form.id,
      fullName: this.form.fullName,
      email: this.form.email,
      phoneNumber: this.form.phoneNumber,
      password: this.form.password,
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
