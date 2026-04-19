import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommentDto } from '../_models/commentDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private baseUrl = '/api/Comment';

  constructor(private http : HttpClient) {}

  private normalizeComment(item: any): CommentDto {
    if (!item || typeof item !== 'object') return item as CommentDto;

    if (item.Id != null && item.id == null) item.id = item.Id;
    if (item.FirstName != null && item.firstName == null) item.firstName = item.FirstName;
    if (item.LastName != null && item.lastName == null) item.lastName = item.LastName;
    if (item.Email != null && item.email == null) item.email = item.Email;
    if (item.Body != null && item.body == null) item.body = item.Body;
    if (item.BlogId != null && item.blogId == null) item.blogId = item.BlogId;
    if (item.CreatedAt != null && item.createdAt == null) item.createdAt = item.CreatedAt;
    if (item.UpdatedAt != null && item.updatedAt == null) item.updatedAt = item.UpdatedAt;
    if (item.CommenterImageUrl != null && item.commenterImageUrl == null) item.commenterImageUrl = item.CommenterImageUrl;
    if (item.commentDate == null) item.commentDate = item.createdAt ?? item.CreatedAt;
    if (item.commenterImageUrl != null) item.commenterImageUrl = this.normalizeUploadsUrl(item.commenterImageUrl);

    if (!Array.isArray(item.subComments)) item.subComments = [];

    return item as CommentDto;
  }

  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeComment(i)))
    );
  }

  getForBlog(blogId: string) {
    return this.http.get<any>(`${this.baseUrl}?$filter=BlogId eq '${blogId}'&$orderby=CreatedAt desc`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeComment(i)))
    );
  }

  create(commentDto:CommentDto){
    return this.http.post<any>(this.baseUrl,commentDto);
  }

  update(model:CommentDto){
    return this.http.put<any>(this.baseUrl,model);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  getById(id:string){
    return this.http.get<any>(`${this.baseUrl}?$filter=Id eq '${id}'&$top=1`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items?.[0] ? this.normalizeComment(items[0]) : null)
    );
  }

  private normalizeUploadsUrl(url: any): string | undefined {
    if (typeof url !== 'string' || !url) return undefined;
    if (/^https?:\/\//i.test(url)) return url;
    if (url.startsWith('/')) return url;
    if (url.startsWith('uploads/')) return `/${url}`;
    return url;
  }
}
