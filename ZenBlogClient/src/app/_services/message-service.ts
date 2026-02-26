import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MessageDto } from '../_models/messageDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

constructor(private http: HttpClient

  ){}

  private baseUrl = '/api/Message';


  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? []))
    );
  }

  getReadMessages(){
    return this.http.get<any>(`${this.baseUrl}?$filter=IsRead eq true`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? []))
    );
  }

  getUnreadMessages(){
    return this.http.get<any>(`${this.baseUrl}?$filter=IsRead eq false`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? []))
    );
  }


  create(model:MessageDto){
    return this.http.post<any>(this.baseUrl,model);
  }

  update(model:MessageDto){
    return this.http.put<any>(this.baseUrl,model);
  }

  delete(id:string){
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  getBlogById(id:string){
    return this.http.get<any>(`${this.baseUrl}?$filter=Id eq '${id}'&$top=1`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items?.[0] ?? null)
    );
  }
}
