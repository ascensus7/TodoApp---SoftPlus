import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateTodoItemRequest,
  PagedResult,
  TodoItem,
  TodoItemQueryParameters,
  UpdateTodoItemRequest
} from '../models/todo-item.models';

@Injectable({
  providedIn: 'root'
})
export class TodoItemService {
  private apiUrl = environment.apiUrl + '/todo-items';

  constructor(private http: HttpClient) {
  }

  getAll(
    query: TodoItemQueryParameters
  ): Observable<PagedResult<TodoItem>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.search !== undefined) {
      params = params.set('search', query.search);
    }

    if (query.categoryId !== undefined) {
      params = params.set('categoryId', query.categoryId);
    }

    if (query.isCompleted !== undefined) {
      params = params.set('isCompleted', query.isCompleted);
    }

    return this.http.get<PagedResult<TodoItem>>(
      this.apiUrl,
      { params }
    );
  }

  getById(id: number): Observable<TodoItem> {
    return this.http.get<TodoItem>(
      `${this.apiUrl}/${id}`
    );
  }

  create(request: CreateTodoItemRequest): Observable<TodoItem> {
    return this.http.post<TodoItem>(
      this.apiUrl,
      request
    );
  }

  update(
    id: number,
    request: UpdateTodoItemRequest
  ): Observable<TodoItem> {
    return this.http.put<TodoItem>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}