export interface TodoItem {
  id: number;
  title: string;
  description: string | null;
  isCompleted: boolean;
  createdAt: string;
  updatedAt: string | null;
  dueDate: string | null;
  categoryId: number | null;
  categoryName: string | null;
}

export interface CreateTodoItemRequest {
  title: string;
  description: string | null;
  dueDate: string | null;
  categoryId: number | null;
}

export interface UpdateTodoItemRequest {
  title: string;
  description: string | null;
  isCompleted: boolean;
  dueDate: string | null;
  categoryId: number | null;
}

export interface TodoItemQueryParameters {
  pageNumber: number;
  pageSize: number;
  search?: string;
  categoryId?: number;
  isCompleted?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}