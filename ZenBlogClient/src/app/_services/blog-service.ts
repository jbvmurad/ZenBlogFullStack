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

  private splitFullName(fullName: any): { firstName?: string; lastName?: string } {
    if (typeof fullName !== 'string' || !fullName.trim()) return {};
    const parts = fullName.trim().split(/\s+/);
    return {
      firstName: parts[0],
      lastName: parts.slice(1).join(' ') || undefined
    };
  }

  private coerceBlogCasing(b: any): any {
    if (!b || typeof b !== 'object') return b;

    if (b.Id != null && b.id == null) b.id = b.Id;
    if (b.Title != null && b.title == null) b.title = b.Title;
    if (b.CoverImage != null && b.coverImage == null) b.coverImage = b.CoverImage;
    if (b.BlogImage != null && b.blogImage == null) b.blogImage = b.BlogImage;
    if (b.Description != null && b.description == null) b.description = b.Description;
    if (b.CategoryId != null && b.categoryId == null) b.categoryId = b.CategoryId;
    if (b.CategoryName != null && b.categoryName == null) b.categoryName = b.CategoryName;
    if (b.UserId != null && b.userId == null) b.userId = b.UserId;
    if (b.UserFullName != null && b.userFullName == null) b.userFullName = b.UserFullName;
    if (b.CreatedAt != null && b.createdAt == null) b.createdAt = b.CreatedAt;
    if (b.UpdatedAt != null && b.updatedAt == null) b.updatedAt = b.UpdatedAt;

    if (b.Category != null && b.category == null) b.category = b.Category;
    if (b.User != null && b.user == null) b.user = b.User;
    if (b.Comments != null && b.comments == null) b.comments = b.Comments;

    if (b.category && typeof b.category === 'object') {
      const c = b.category;
      if (c.Id != null && c.id == null) c.id = c.Id;
      if (c.CategoryName != null && c.categoryName == null) c.categoryName = c.CategoryName;
      if (c.Name != null && c.categoryName == null) c.categoryName = c.Name;
      if (c.name != null && c.categoryName == null) c.categoryName = c.name;
    }

    return b;
  }

  private normalizeUploadsUrl(url: any): any {
    if (typeof url !== 'string' || !url) return url;
    if (/^https?:\/\//i.test(url)) return url;
    if (url.startsWith('/')) return url;
    if (url.startsWith('uploads/')) return `/${url}`;
    return url;
  }

  private normalizeBlog(b: any): BlogDto {
    b = this.coerceBlogCasing(b);
    b.coverImage = this.normalizeUploadsUrl(b.coverImage);
    b.blogImage = this.normalizeUploadsUrl(b.blogImage);

    if (!b.category && (b.categoryId != null || b.categoryName != null)) {
      b.category = {
        id: b.categoryId,
        categoryName: b.categoryName
      };
    } else if (b?.category && b.category.categoryName == null) {
      b.category.categoryName = b.category.name ?? b.category.Name ?? b.categoryName;
    }

    if (!b.user && (b.userId != null || b.userFullName != null)) {
      const split = this.splitFullName(b.userFullName);
      b.user = {
        id: b.userId,
        fullName: b.userFullName,
        firstName: split.firstName,
        lastName: split.lastName
      };
    } else if (b.user && b.user.fullName == null && b.userFullName != null) {
      const split = this.splitFullName(b.userFullName);
      b.user.fullName = b.userFullName;
      b.user.firstName ??= split.firstName;
      b.user.lastName ??= split.lastName;
    }

    if (!b.user) b.user = {};
    if (!b.category) b.category = {};
    if (!Array.isArray(b.comments)) {
      b.comments = [];
    }

    return b as BlogDto;
  }

  private buildFormData(model: Partial<BlogDto> & { id?: string }, coverFile?: File | null, blogFile?: File | null): FormData {
    const form = new FormData();

    if (model?.id != null) form.append('Id', model.id);
    if (model?.title != null) form.append('Title', model.title);
    if (model?.description != null) form.append('Description', model.description);
    if (model?.categoryId != null) form.append('CategoryId', model.categoryId);
    if (model?.userId != null) form.append('UserId', model.userId);
    if (coverFile) form.append('CoverImage', coverFile);
    if (blogFile) form.append('BlogImage', blogFile);

    return form;
  }

  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeBlog(i)))
    );
  }

  getLatest5Blogs(){
    return this.http.get<any>(`${this.baseUrl}?$orderby=CreatedAt desc&$top=5`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeBlog(i)))
    );
  }

  create(model: BlogDto, coverFile: File, blogFile: File){
    const form = this.buildFormData(model, coverFile, blogFile);
    return this.http.post<any>(this.baseUrl, form);
  }

  update(model: Partial<BlogDto> & { id: string }, coverFile?: File | null, blogFile?: File | null){
    const form = this.buildFormData(model, coverFile, blogFile);
    return this.http.put<any>(this.baseUrl, form);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  getBlogById(id:string){
    return this.http.get<any>(`${this.baseUrl}?$filter=Id eq '${id}'&$top=1`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => (items?.[0] ? this.normalizeBlog(items[0]) : null))
    );
  }
}
