import { HttpClient } from '@angular/common/http';
//Displays class like as a service that can be injected into other componens
import { Injectable } from '@angular/core';
//Import Observable from rxjs library to handle asynchronous data streams
import { Observable } from 'rxjs';
//Import environment variables from environment.ts file
import { environment } from '../../../environments/environment';
import {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest
} from '../models/category.models';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = environment.apiUrl + '/categories';

  constructor(private http: HttpClient) {
  }

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.apiUrl);
  }

  getById(id: number): Observable<Category> {
    const url = this.apiUrl + '/' + id;

    return this.http.get<Category>(url);
  }

  create(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, request);
  }

  update(
    id: number,
    request: UpdateCategoryRequest
  ): Observable<Category> {
    const url = this.apiUrl + '/' + id;

    return this.http.put<Category>(url, request);
  }

  delete(id: number): Observable<void> {
    const url = this.apiUrl + '/' + id;

    return this.http.delete<void>(url);
  }
}