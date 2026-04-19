import { Component } from '@angular/core';
import { RoleDto, RoleService } from '../../_services/role-service';
import { UserDto, UserService } from '../../_services/user-service';
import { UserRoleDto, UserRoleService } from '../../_services/user-role-service';
import { AuthService } from '../../_services/auth-service';

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
  query = '';
  newRoleName = '';
  selectedRoleByUser: Record<string, string> = {};

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private userRoleService: UserRoleService,
    private authService: AuthService
  ) {
    this.load();
  }

  load() {
    this.loading = true;

    this.roleService.getAll().subscribe({
      next: (roles) => {
        this.roles = roles;

        this.userService.getAll().subscribe({
          next: (users) => {
            this.users = users;

            this.userRoleService.getAll().subscribe({
              next: (userRoles) => {
                this.userRoles = userRoles;
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
        this.loading = false;
      }
    });
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

  initials(user: UserDto): string {
    const name = user.fullName || user.userName || user.email || 'U';
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(x => x[0])
      .join('')
      .toUpperCase();
  }

  rolesForUser(userId: string): RoleDto[] {
    const assignedRoleIds = new Set(
      (this.userRoles ?? [])
        .filter(x => x.userId === userId)
        .map(x => x.roleId)
    );

    return (this.roles ?? []).filter(role => assignedRoleIds.has(role.id));
  }

  availableRolesForUser(userId: string): RoleDto[] {
    const user = (this.users ?? []).find(x => x.id === userId);
    if (this.isProtectedDashboardAdminUser(user)) {
      return [];
    }

    const assignedRoleIds = new Set(this.rolesForUser(userId).map(x => x.id));
    return (this.roles ?? []).filter(role =>
      !assignedRoleIds.has(role.id) &&
      (role?.name ?? '').trim().toLowerCase() !== 'admin');
  }

  roleUsageCount(roleId: string): number {
    return (this.userRoles ?? []).filter(x => x.roleId === roleId).length;
  }

  createRole() {
    if (!this.canManageWorkers()) return;

    const name = (this.newRoleName ?? '').trim();
    if (!name || this.busy) return;

    const exists = (this.roles ?? []).some(role => (role.name ?? '').trim().toLowerCase() === name.toLowerCase());
    if (exists) {
      try { alertify?.error?.('This role already exists.'); } catch {}
      return;
    }

    this.busy = true;
    this.roleService.create(name).subscribe({
      next: (res: any) => {
        this.newRoleName = '';
        try { alertify?.success?.(res?.message ?? 'Role created.'); } catch {}
        this.finishMutation(true);
      },
      error: (err) => this.handleError(err, 'Failed to create role.')
    });
  }

  deleteRole(role: RoleDto) {
    if (!this.canManageWorkers()) return;
    if (this.busy || !role?.id) return;

    const accepted = window.confirm(`Delete role "${role.name ?? 'role'}"?`);
    if (!accepted) return;

    this.busy = true;
    this.roleService.delete(role.id).subscribe({
      next: (res: any) => {
        try { alertify?.success?.(res?.message ?? 'Role deleted.'); } catch {}
        this.finishMutation(false);
      },
      error: (err) => this.handleError(err, 'Failed to delete role.')
    });
  }

  assignRole(user: UserDto) {
    if (!this.canManageWorkers()) return;
    const userId = user?.id;
    const roleId = userId ? this.selectedRoleByUser[userId] : null;
    if (!userId || !roleId || this.busy) return;

    this.busy = true;
    this.userRoleService.giveRole(userId, roleId).subscribe({
      next: (res: any) => {
        this.selectedRoleByUser[userId] = '';
        try { alertify?.success?.(res?.message ?? 'Role assigned.'); } catch {}
        this.finishMutation(true);
      },
      error: (err) => this.handleError(err, 'Failed to assign role.')
    });
  }

  removeRole(user: UserDto, role: RoleDto) {
    if (!this.canManageWorkers()) return;
    if (!user?.id || !role?.id || this.busy) return;

    this.busy = true;
    this.userRoleService.deleteRoles(user.id, [role.id]).subscribe({
      next: (res: any) => {
        try { alertify?.success?.(res?.message ?? 'Role removed.'); } catch {}
        this.finishMutation(true);
      },
      error: (err) => this.handleError(err, 'Failed to remove role.')
    });
  }

  canRemoveRole(user: UserDto, role: RoleDto): boolean {
    const isProtectedDashboardAdmin = user?.isProtectedDashboardAdmin === true;
    const isAdminRole = (role?.name ?? '').trim().toLowerCase() === 'admin';
    return !(isProtectedDashboardAdmin && isAdminRole);
  }

  canDeleteRole(role: RoleDto): boolean {
    return (role?.name ?? '').trim().toLowerCase() !== 'admin';
  }

  canManageWorkers(): boolean {
    return this.authService.isAdmin();
  }

  isManagerReadonly(): boolean {
    return this.authService.hasDashboardAccess() && !this.authService.isAdmin();
  }

  isProtectedDashboardAdminUser(user: UserDto | null | undefined): boolean {
    return user?.isProtectedDashboardAdmin === true;
  }

  canAssignNewRole(user: UserDto): boolean {
    return this.canManageWorkers() && !this.isProtectedDashboardAdminUser(user);
  }

  trackByUser(_: number, user: UserDto) {
    return user.id;
  }

  trackByRole(_: number, role: RoleDto) {
    return role.id;
  }

  private finishMutation(refreshAdminStatus: boolean) {
    this.busy = false;
    if (refreshAdminStatus) {
      this.authService.refreshRoleAccess().subscribe();
    }
    this.load();
  }

  private handleError(err: any, fallback: string) {
    console.error(err);
    this.busy = false;
    const msg = err?.error?.message ?? err?.error?.Message ?? err?.message ?? fallback;
    try { alertify?.error?.(msg); } catch {}
  }
}
