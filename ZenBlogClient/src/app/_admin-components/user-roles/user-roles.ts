import { Component } from '@angular/core';
import { RoleDto, RoleService } from '../../_services/role-service';
import { UserDto, UserService } from '../../_services/user-service';
import { UserRoleDto, UserRoleService } from '../../_services/user-role-service';
import { AccessControlService } from '../../_services/access-control-service';

declare const alertify: any;

@Component({
  selector: 'admin-user-roles',
  standalone: false,
  templateUrl: './user-roles.html',
  styleUrl: './user-roles.css'
})
export class UserRoles {
  loading = true;
  busy = false;

  users: UserDto[] = [];
  roles: RoleDto[] = [];
  userRoles: UserRoleDto[] = [];

  adminRoleId: string | null = null;
  query = '';

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private userRoleService: UserRoleService,
    private access: AccessControlService
  ) {
    this.load();
  }

  load() {
    this.loading = true;

    // Fetch everything in parallel
    this.roleService.getAll().subscribe({
      next: (roles) => {
        this.roles = roles;
        this.adminRoleId = this.findAdminRoleId(roles);

        // Users + userRoles depend on token only
        this.userService.getAll().subscribe({
          next: (users) => {
            this.users = users;
            this.userRoleService.getAll().subscribe({
              next: (urs) => {
                this.userRoles = urs;
                this.loading = false;
              },
              error: (err) => {
                console.error(err);
                this.userRoles = [];
                this.loading = false;
              }
            });
          },
          error: (err) => {
            console.error(err);
            this.users = [];
            this.loading = false;
          }
        });
      },
      error: (err) => {
        console.error(err);
        this.roles = [];
        this.adminRoleId = null;
        this.loading = false;
      }
    });
  }

  private findAdminRoleId(roles: RoleDto[]): string | null {
    const admin = roles.find(r => (r?.name ?? '').toLowerCase().includes('admin'));
    return admin?.id ?? null;
  }

  filteredUsers(): UserDto[] {
    const q = (this.query ?? '').trim().toLowerCase();
    if (!q) return this.users;
    return (this.users ?? []).filter(u =>
      (u.fullName ?? '').toLowerCase().includes(q) ||
      (u.userName ?? '').toLowerCase().includes(q) ||
      (u.email ?? '').toLowerCase().includes(q)
    );
  }

  isUserAdmin(userId: string): boolean {
    if (!this.adminRoleId) return false;
    return (this.userRoles ?? []).some(ur => ur.userId === userId && ur.roleId === this.adminRoleId);
  }

  async createAdminRole() {
    if (this.busy) return;
    this.busy = true;
    this.roleService.create('Admin').subscribe({
      next: (res: any) => {
        try { alertify?.success?.(res?.message ?? 'Admin role created.'); } catch {}
        this.busy = false;
        this.load();
      },
      error: (err) => {
        console.error(err);
        this.busy = false;
        const msg = err?.error?.message ?? err?.error?.Message ?? err?.message ?? 'Failed to create role.';
        try { alertify?.error?.(msg); } catch {}
      }
    });
  }

  toggleAdmin(user: UserDto, checked: boolean) {
    if (!this.adminRoleId || this.busy) return;
    const userId = user.id;
    if (!userId) return;

    this.busy = true;
    if (checked) {
      this.userRoleService.giveRole(userId, this.adminRoleId).subscribe({
        next: (res: any) => {
          try { alertify?.success?.(res?.message ?? 'Role granted.'); } catch {}
          this.busy = false;
          this.load();
          // In case we changed the current user's role, refresh access state
          this.access.ensureLoaded(true).subscribe();
        },
        error: (err) => {
          console.error(err);
          this.busy = false;
          const msg = err?.error?.message ?? err?.error?.Message ?? err?.message ?? 'Failed to grant role.';
          try { alertify?.error?.(msg); } catch {}
        }
      });
    } else {
      this.userRoleService.deleteRoles(userId, [this.adminRoleId]).subscribe({
        next: (res: any) => {
          try { alertify?.success?.(res?.message ?? 'Role removed.'); } catch {}
          this.busy = false;
          this.load();
          this.access.ensureLoaded(true).subscribe();
        },
        error: (err) => {
          console.error(err);
          this.busy = false;
          const msg = err?.error?.message ?? err?.error?.Message ?? err?.message ?? 'Failed to remove role.';
          try { alertify?.error?.(msg); } catch {}
        }
      });
    }
  }
}
