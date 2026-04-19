import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

export interface UserRoleDto {
  id?: string;
  userId: string;
  roleId: string;
  role?: { id?: string; name?: string };
  roleName?: string;
  userFullName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {
  private baseUrl = '/api/UserRole';

  constructor(private http: HttpClient) {}

  private normalize(ur: any): UserRoleDto {
    if (!ur || typeof ur !== 'object') return ur as UserRoleDto;
    if (ur.Id != null && ur.id == null) ur.id = ur.Id;
    if (ur.UserId != null && ur.userId == null) ur.userId = ur.UserId;
    if (ur.RoleId != null && ur.roleId == null) ur.roleId = ur.RoleId;
    if (ur.RoleName != null && ur.roleName == null) ur.roleName = ur.RoleName;
    if (ur.UserFullName != null && ur.userFullName == null) ur.userFullName = ur.UserFullName;

    const roleName = ur.roleName ?? ur.RoleName ?? ur.role?.name ?? ur.Role?.Name ?? ur.role?.Name ?? ur.Role?.name;
    if (roleName && ur.role == null) {
      ur.role = {
        id: ur.roleId,
        name: roleName
      };
    }

    return ur as UserRoleDto;
  }

  getAll() {
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  getForUser(userId: string) {
    const params = new HttpParams().set('$filter', `UserId eq '${userId}'`);
    return this.http.get<any>(this.baseUrl, { params }).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  giveRole(userId: string, roleId: string) {
    return this.http.post<any>(this.baseUrl, { UserId: userId, RoleId: roleId });
  }

  deleteRoles(userId: string, roleIds: string[]) {
    return this.http.delete<any>(`${this.baseUrl}/${encodeURIComponent(userId)}`, {
      body: { RoleIds: roleIds }
    });
  }
}
