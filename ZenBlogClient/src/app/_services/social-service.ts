import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SocialDto } from '../_models/socialDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SocialService {
  constructor(private http: HttpClient) {}

  private baseUrl = '/api/Social';

  private coerceSocialCasing(s: any): any {
    if (!s || typeof s !== 'object') return s;
    if (s.Id != null && s.id == null) s.id = s.Id;
    if (s.Title != null && s.title == null) s.title = s.Title;
    if (s.Url != null && s.url == null) s.url = s.Url;
    if (s.Icon != null && s.icon == null) s.icon = s.Icon;
    return s;
  }

  private normalizeUploadsUrl(url: any): any {
    if (typeof url !== 'string' || !url) return url;
    if (/^https?:\/\//i.test(url)) return url;
    if (url.startsWith('/')) return url;
    if (url.startsWith('uploads/')) return `/${url}`;
    return url;
  }

  private normalizeSocial(s: any): SocialDto {
    s = this.coerceSocialCasing(s);
    s.icon = this.normalizeUploadsUrl(s.icon);
    return s as SocialDto;
  }

  private buildFormData(model: Partial<SocialDto> & { id?: string }, iconFile?: File | null): FormData {
    const form = new FormData();
    if (model?.id != null) form.append('Id', model.id);
    if (model?.title != null) form.append('Title', model.title);
    if (model?.url != null) form.append('Url', model.url);
    if (iconFile) form.append('Icon', iconFile);
    return form;
  }

  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeSocial(i)))
    );
  }

  createWithMedia(model: Partial<SocialDto>, iconFile: File){
    const form = this.buildFormData(model, iconFile);
    return this.http.post<any>(this.baseUrl, form);
  }

  updateWithMedia(model: Partial<SocialDto> & { id: string }, iconFile?: File | null){
    const form = this.buildFormData(model, iconFile);
    return this.http.put<any>(this.baseUrl, form);
  }

  create(model: SocialDto, iconFile?: File | null){
    const form = this.buildFormData(model, iconFile);
    return this.http.post<any>(this.baseUrl, form);
  }

  update(model: SocialDto, iconFile?: File | null){
    const form = this.buildFormData(model, iconFile);
    return this.http.put<any>(this.baseUrl, form);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }
}
