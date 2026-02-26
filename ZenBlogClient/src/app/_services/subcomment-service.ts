import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SubCommentDto } from '../_models/subCommentDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SubCommentService {
  constructor(private http: HttpClient

  ){}

  private baseUrl = '/api/SubComment';


  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? []))
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

  // NOTE: ZenBlogServer tarafında SubComment için "GetById" endpoint'i yok.
}
