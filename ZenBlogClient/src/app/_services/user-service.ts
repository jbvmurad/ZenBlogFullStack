import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

export interface UserDto {
  id: string;
  userName?: string;
  email?: string;
  phoneNumber?: string;
  fullName?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = '/api/Auth';

  constructor(private http: HttpClient) {}

  private normalize(u: any): UserDto {
    if (!u || typeof u !== 'object') return u as UserDto;

    // OData casing (PascalCase) -> UI casing
    if (u.Id != null && u.id == null) u.id = u.Id;
    if (u.UserName != null && u.userName == null) u.userName = u.UserName;
    if (u.Email != null && u.email == null) u.email = u.Email;
    if (u.PhoneNumber != null && u.phoneNumber == null) u.phoneNumber = u.PhoneNumber;
    if (u.FullName != null && u.fullName == null) u.fullName = u.FullName;

    return u as UserDto;
  }

  /** GET /api/Auth (OData) */
  getAll() {
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  /** GET /api/Auth?$filter=Id eq '...' (OData) */
  getById(id: string) {
    const params = new HttpParams().set('$filter', `Id eq '${id}'`);
    return this.http.get<any>(this.baseUrl, { params }).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => (items.length > 0 ? this.normalize(items[0]) : null))
    );
  }

  /** PUT /api/Auth */
  update(payload: { Id: string; FullName?: string | null; Email?: string | null; PhoneNumber?: string | null; Password?: string | null }) {
    return this.http.put<any>(this.baseUrl, payload);
  }

  /** DELETE /api/Auth?id=... */
  delete(id: string) {
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }
}
