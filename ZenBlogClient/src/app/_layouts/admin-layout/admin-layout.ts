import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../_services/auth-service';
import { UserDto } from '../../_models/userDto';

@Component({
  selector: 'admin-layout',
  standalone: false,
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css'
})
export class AdminLayout implements OnInit {
  currentUser: UserDto | null = null;

  constructor(private authService: AuthService){}

  ngOnInit(): void {
    this.authService.getCurrentUser(true).subscribe(user => this.currentUser = user);
    this.authService.currentUser$.subscribe(user => this.currentUser = user);
    this.authService.refreshRoleAccess().subscribe();
  }

  getUserName(){
    return this.currentUser?.fullName || this.currentUser?.userName || this.authService.getUserName() || 'Admin user';
  }

  get userImage() {
    return this.currentUser?.imageUrl || null;
  }

  get initials() {
    return this.getUserName()
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(x => x[0])
      .join('')
      .toUpperCase();
  }

  get dashboardRoleTitle() {
    return this.authService.isAdmin() ? 'Administrator area' : 'Manager area';
  }

  logout(){
    this.authService.logout();
  }
}
