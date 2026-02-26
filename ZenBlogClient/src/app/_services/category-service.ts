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

    // OData casing (PascalCase) -> UI casing (camelCase)
    if (c.Id != null && c.id == null) c.id = c.Id;

    // Backend entity is `Name` but UI expects `categoryName`
    if (c.categoryName == null) {
      c.categoryName = c.name ?? c.Name ?? c.CategoryName;
    }

    // Also normalize nested blogs if they come expanded in future
    if (c.Blogs != null && c.blogs == null) c.blogs = c.Blogs;

    return c as CategoryDto;
  }

  /**
   * GET api/Category
   * Supports both plain array and OData-style { value: [...] } responses.
   */
  getCategories() {
    return this.http.get<any>(this.baseUrl).pipe(
      map((res: any) => (Array.isArray(res) ? res : res?.value ?? [])),
      map((items: any[]) => items.map(i => this.normalizeCategory(i)))
    );
  }

  /** POST api/Category */
  create(categoryDto: CategoryDto) {
    const payload = { Name: categoryDto.categoryName };
    return this.http.post<any>(this.baseUrl, payload);
  }

  /** DELETE api/Category?id=... */
  delete(id: string) {
    return this.http.delete<any>(`${this.baseUrl}?id=${encodeURIComponent(id)}`);
  }

  /** PUT api/Category */
  update(model: CategoryDto) {
    const payload = { id: model.id, Name: model.categoryName };
    return this.http.put<any>(this.baseUrl, payload);
  }

}
