import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ContactInfoDto } from '../_models/contactInfoDto';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ContactInfoService {


   constructor(private http: HttpClient){}

  private baseUrl = '/api/ContactInfo';


getAll(){
  return this.http.get<any>(this.baseUrl).pipe(
    map((res: any) => (Array.isArray(res) ? res : res?.value ?? []))
  );
}

create(model:ContactInfoDto){
  return this.http.post<any>(this.baseUrl,model);
}

update(model:ContactInfoDto){
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
