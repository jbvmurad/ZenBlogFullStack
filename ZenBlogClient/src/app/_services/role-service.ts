import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

export interface RoleDto {
  id: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private baseUrl = '/api/Role';

  constructor(private http: HttpClient) {}

  private normalize(r: any): RoleDto {
    if (!r || typeof r !== 'object') return r as RoleDto;
    if (r.Id != null && r.id == null) r.id = r.Id;
    if (r.Name != null && r.name == null) r.name = r.Name;
    return r as RoleDto;
  }

  getAll() {
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  create(name: string) {
    return this.http.post<any>(this.baseUrl, { Name: name });
  }

  delete(id: string) {
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }
}
