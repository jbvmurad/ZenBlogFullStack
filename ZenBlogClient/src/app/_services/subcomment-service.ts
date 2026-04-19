import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SubCommentDto } from '../_models/subCommentDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SubCommentService {
  constructor(private http: HttpClient){}

  private baseUrl = '/api/SubComment';

  private normalize(item: any): SubCommentDto {
    if (!item || typeof item !== 'object') return item as SubCommentDto;
    if (item.Id != null && item.id == null) item.id = item.Id;
    if (item.FirstName != null && item.firstName == null) item.firstName = item.FirstName;
    if (item.LastName != null && item.lastName == null) item.lastName = item.LastName;
    if (item.Email != null && item.email == null) item.email = item.Email;
    if (item.Body != null && item.body == null) item.body = item.Body;
    if (item.CommentId != null && item.commentId == null) item.commentId = item.CommentId;
    if (item.CreatedAt != null && item.createdAt == null) item.createdAt = item.CreatedAt;
    if (item.UpdatedAt != null && item.updatedAt == null) item.updatedAt = item.UpdatedAt;
    if (item.commentDate == null) item.commentDate = item.createdAt ?? item.CreatedAt;
    return item as SubCommentDto;
  }

  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  create(model:SubCommentDto){
    return this.http.post<any>(this.baseUrl,model);
  }

  update(model:SubCommentDto){
    return this.http.put<any>(this.baseUrl,model);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }
}
