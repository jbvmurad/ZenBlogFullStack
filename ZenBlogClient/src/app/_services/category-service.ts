import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CategoryDto } from '../_models/category';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private baseUrl = '/api/Category';

  constructor(private http: HttpClient) {}

  private normalizeCategory(c: any): CategoryDto {
    if (!c || typeof c !== 'object') return c as CategoryDto;

    if (c.Id != null && c.id == null) c.id = c.Id;

    if (c.categoryName == null) {
      c.categoryName = c.name ?? c.Name ?? c.CategoryName;
    }

    if (c.Blogs != null && c.blogs == null) c.blogs = c.Blogs;

    return c as CategoryDto;
  }

  getCategories() {
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeCategory(i)))
    );
  }

  create(categoryDto: CategoryDto) {
    const payload = { Name: categoryDto.categoryName };
    return this.http.post<any>(this.baseUrl, payload);
  }

  delete(id: string) {
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  update(model: CategoryDto) {
    const payload = { id: model.id, Name: model.categoryName };
    return this.http.put<any>(this.baseUrl, payload);
  }

}
