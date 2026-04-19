import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { UserDto } from '../../_models/userDto';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {
  user: UserDto | null = null;
  loading = true;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.getCurrentUser(true).subscribe({
      next: user => {
        this.user = user;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
    this.authService.refreshRoleAccess().subscribe();
  }

  get displayName(): string {
    return this.user?.fullName || this.user?.userName || 'User';
  }

  get initials(): string {
    return this.displayName
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(x => x[0])
      .join('')
      .toUpperCase();
  }

  isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  hasDashboardAccess(): boolean {
    return this.authService.hasDashboardAccess();
  }

  get roleLabel(): string | null {
    if (this.authService.isAdmin()) return 'Admin';
    if (this.authService.isManager()) return 'Manager';
    return null;
  }
}
