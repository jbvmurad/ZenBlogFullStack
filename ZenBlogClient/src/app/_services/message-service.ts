import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MessageDto } from '../_models/messageDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private http: HttpClient){}

  private baseUrl = '/api/Message';

  private normalize(item: any): MessageDto {
    if (!item || typeof item !== 'object') return item as MessageDto;
    if (item.Id != null && item.id == null) item.id = item.Id;
    if (item.Name != null && item.name == null) item.name = item.Name;
    if (item.Email != null && item.email == null) item.email = item.Email;
    if (item.Subject != null && item.subject == null) item.subject = item.Subject;
    if (item.MessageBody != null && item.messageBody == null) item.messageBody = item.MessageBody;
    if (item.IsRead != null && item.isRead == null) item.isRead = item.IsRead;
    return item as MessageDto;
  }

  getAll(){
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  getReadMessages(){
    return this.http.get<any>(`${this.baseUrl}?$filter=IsRead eq true`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
    );
  }

  getUnreadMessages(){
    return this.http.get<any>(`${this.baseUrl}?$filter=IsRead eq false`).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalize(i)))
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
      map((items: any[]) => items?.[0] ? this.normalize(items[0]) : null)
    );
  }
}
