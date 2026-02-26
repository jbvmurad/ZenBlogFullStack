import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SocialDto } from '../_models/socialDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SocialService {
  constructor(private http: HttpClient

  ){}

  private baseUrl = '/api/Social';

  /** Coerce OData/PascalCase payloads to the UI's camelCase shape. */
  private coerceSocialCasing(s: any): any {
    if (!s || typeof s !== 'object') return s;
    if (s.Id != null && s.id == null) s.id = s.Id;
    if (s.Title != null && s.title == null) s.title = s.Title;
    if (s.Url != null && s.url == null) s.url = s.Url;
    if (s.Icon != null && s.icon == null) s.icon = s.Icon;
    return s;
  }

  /** Ensure /uploads/... urls work with the Angular dev proxy. */
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


  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeSocial(i)))
    );
  }

  /**
   * Store-style create: send icon as multipart/form-data and other fields as query params.
   * Backend endpoint: POST /api/Social/with-media
   */
  createWithMedia(model: Partial<SocialDto>, iconFile: File){
    const form = new FormData();
    form.append('Icon', iconFile);

    const url = `${this.baseUrl}/with-media`
      + `?Title=${encodeURIComponent(model?.title ?? '')}`
      + `&Url=${encodeURIComponent(model?.url ?? '')}`;

    return this.http.post<any>(url, form);
  }

  /**
   * Store-style update: optional icon. If not sent, server keeps the old one.
   * Backend endpoint: PUT /api/Social/with-media
   */
  updateWithMedia(model: Partial<SocialDto> & { id: string }, iconFile?: File | null){
    const form = new FormData();
    if (iconFile) form.append('Icon', iconFile);

    const qs: string[] = [`id=${encodeURIComponent(model?.id ?? '')}`];
    if (model?.title != null && model.title !== '') qs.push(`Title=${encodeURIComponent(model.title)}`);
    if (model?.url != null && model.url !== '') qs.push(`Url=${encodeURIComponent(model.url)}`);

    const url = `${this.baseUrl}/with-media?${qs.join('&')}`;
    return this.http.put<any>(url, form);
  }

  // Backward-compatible JSON endpoints (kept for older API versions)
  create(model:SocialDto){
    return this.http.post<any>(this.baseUrl, model);
  }

  update(model:SocialDto){
    return this.http.put<any>(this.baseUrl, model);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  // NOTE: ZenBlogServer tarafında Social için "GetById" endpoint'i yok.
  // Gerekirse OData ile filtrelenebilir: /api/Social?$filter=Id eq '...'
}
