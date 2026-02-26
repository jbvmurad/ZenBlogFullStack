import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BlogDto } from '../_models/blog';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BlogService {

  constructor(private http: HttpClient){}

  private baseUrl = '/api/Blog';

  /**
   * The backend uses OData, which serializes entity/property names with the
   * same casing as the EDM model (PascalCase: Id, Title, CoverImage...).
   * The UI models/components expect camelCase (id, title, coverImage...).
   */
  private coerceBlogCasing(b: any): any {
    if (!b || typeof b !== 'object') return b;

    // Top-level Blog
    if (b.Id != null && b.id == null) b.id = b.Id;
    if (b.Title != null && b.title == null) b.title = b.Title;
    if (b.CoverImage != null && b.coverImage == null) b.coverImage = b.CoverImage;
    if (b.BlogImage != null && b.blogImage == null) b.blogImage = b.BlogImage;
    if (b.Description != null && b.description == null) b.description = b.Description;
    if (b.CategoryId != null && b.categoryId == null) b.categoryId = b.CategoryId;
    if (b.UserId != null && b.userId == null) b.userId = b.UserId;
    if (b.CreatedAt != null && b.createdAt == null) b.createdAt = b.CreatedAt;
    if (b.UpdatedAt != null && b.updatedAt == null) b.updatedAt = b.UpdatedAt;

    // Navigation properties (OData often returns PascalCase)
    if (b.Category != null && b.category == null) b.category = b.Category;
    if (b.User != null && b.user == null) b.user = b.User;
    if (b.Comments != null && b.comments == null) b.comments = b.Comments;

    // Nested Category
    if (b.category && typeof b.category === 'object') {
      const c = b.category;
      if (c.Id != null && c.id == null) c.id = c.Id;

      // API Category entity is `Name`, UI expects `categoryName`
      if (c.CategoryName != null && c.categoryName == null) c.categoryName = c.CategoryName;
      if (c.Name != null && c.categoryName == null) c.categoryName = c.Name;
      if (c.name != null && c.categoryName == null) c.categoryName = c.name;
    }

    return b;
  }

  /** Ensure /uploads/... urls work with the Angular dev proxy. */
  private normalizeUploadsUrl(url: any): any {
    if (typeof url !== 'string' || !url) return url;
    // keep absolute urls untouched
    if (/^https?:\/\//i.test(url)) return url;
    if (url.startsWith('/')) return url;
    if (url.startsWith('uploads/')) return `/${url}`;
    return url;
  }

  private normalizeBlog(b: any): BlogDto {
    b = this.coerceBlogCasing(b);

    // Make stored paths more forgiving
    b.coverImage = this.normalizeUploadsUrl(b.coverImage);
    b.blogImage = this.normalizeUploadsUrl(b.blogImage);

    // Normalize nested Category name to UI's `categoryName` (extra safety)
    if (b?.category && b.category.categoryName == null && (b.category.name != null || b.category.Name != null)) {
      b.category.categoryName = b.category.name ?? b.category.Name;
    }
    return b as BlogDto;
  }

  getAll(){
    // Expand Category so admin/blog UI can show category name
    return this.http.get<any>(`${this.baseUrl}?$expand=Category`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeBlog(i)))
    );
  }

  getLatest5Blogs(){
    // OData query for latest 5
    return this.http.get<any>(`${this.baseUrl}?$expand=Category&$orderby=CreatedAt desc&$top=5`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeBlog(i)))
    );
  }

  /**
   * Store-style create: send images as multipart/form-data and other fields as query params.
   */
  create(model: BlogDto, coverFile: File, blogFile: File){
    const form = new FormData();
    form.append('CoverImage', coverFile);
    form.append('BlogImage', blogFile);

    const url = `${this.baseUrl}/with-media`
      + `?Title=${encodeURIComponent(model?.title ?? '')}`
      + `&Description=${encodeURIComponent(model?.description ?? '')}`
      + `&CategoryId=${encodeURIComponent(model?.categoryId ?? '')}`
      + `&UserId=${encodeURIComponent(model?.userId ?? '')}`;

    return this.http.post<any>(url, form);
  }

  /**
   * Store-style update: optional images. If a file is not sent, server keeps the old one.
   */
  update(model: Partial<BlogDto> & { id: string }, coverFile?: File | null, blogFile?: File | null){
    const form = new FormData();
    if (coverFile) form.append('CoverImage', coverFile);
    if (blogFile) form.append('BlogImage', blogFile);

    // Only send the fields that are provided; server preserves omitted/empty ones.
    const qs: string[] = [`id=${encodeURIComponent(model?.id ?? '')}`];
    if (model?.title != null && model.title !== '') qs.push(`Title=${encodeURIComponent(model.title)}`);
    if (model?.description != null && model.description !== '') qs.push(`Description=${encodeURIComponent(model.description)}`);
    if (model?.categoryId != null && model.categoryId !== '') qs.push(`CategoryId=${encodeURIComponent(model.categoryId)}`);
    if (model?.userId != null && model.userId !== '') qs.push(`UserId=${encodeURIComponent(model.userId)}`);

    const url = `${this.baseUrl}/with-media?${qs.join('&')}`;

    return this.http.put<any>(url, form);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  getBlogById(id:string){
    // Try to fetch by filter (works with OData query options)
    return this.http.get<any>(`${this.baseUrl}?$expand=Category&$filter=Id eq '${id}'&$top=1`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => (items?.[0] ? this.normalizeBlog(items[0]) : null))
    );
  }
}
