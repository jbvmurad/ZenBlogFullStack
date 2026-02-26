import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CommentDto } from '../_models/commentDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private baseUrl = '/api/Comment';


  constructor(private http : HttpClient) {

}


getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? []))
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
      map((items: any[]) => items?.[0] ?? null)
    );
  }


}
