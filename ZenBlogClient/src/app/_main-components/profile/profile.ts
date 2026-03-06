import { Component } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { UserService, UserDto } from '../../_services/user-service';

@Component({
  selector: 'app-profile',
  standalone: false,
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {
  loading = true;
  user: UserDto | null = null;
  error: string | null = null;

  constructor(private auth: AuthService, private userService: UserService) {
    this.load();
  }

  private load() {
    this.loading = true;
    this.error = null;

    const userId = this.auth.getUserId();
    if (!userId) {
      this.loading = false;
      this.error = 'User not found in token.';
      return;
    }

    this.userService.getById(userId).subscribe({
      next: (u) => {
        this.user = u;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.user = null;
        this.loading = false;
        this.error = 'Failed to load profile.';
      }
    });
  }
}
