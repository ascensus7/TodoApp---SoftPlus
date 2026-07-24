import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Category } from '../../../core/models/category.models';
import {
  TodoItem,
  TodoItemQueryParameters
} from '../../../core/models/todo-item.models';
import { CategoryService } from '../../../core/services/category.service';
import { TodoItemService } from '../../../core/services/todo-item.service';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent implements OnInit {
  tasks: TodoItem[] = [];
  categories: Category[] = [];

  search = '';
  selectedCategoryId: number | null = null;
  selectedCompletion = '';

  pageNumber = 1;
  pageSize = 5;
  totalCount = 0;
  totalPages = 0;

  isLoading = false;
  errorMessage = '';

  constructor(
    private todoItemService: TodoItemService,
    private categoryService: CategoryService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const query: TodoItemQueryParameters = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };

    if (this.search.trim() !== '') {
      query.search = this.search.trim();
    }

    if (this.selectedCategoryId !== null) {
      query.categoryId = this.selectedCategoryId;
    }

    if (this.selectedCompletion !== '') {
      query.isCompleted = this.selectedCompletion === 'true';
    }

    this.todoItemService.getAll(query)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        })
      )
      .subscribe({
        next: result => {
          this.tasks = result.items ?? [];
          this.pageNumber = result.pageNumber;
          this.pageSize = result.pageSize;
          this.totalCount = result.totalCount;
          this.totalPages = result.totalPages;
        },

        error: (error: HttpErrorResponse) => {
          if (error.status === 401) {
            this.errorMessage =
              'Your session has expired. Please log in again.';
          } else {
            this.errorMessage = 'Unable to load tasks.';
          }
        }
      });
  }

  loadCategories(): void {
  this.categoryService.getAll().subscribe({
    next: categories => {
      this.categories = categories;
      this.changeDetectorRef.detectChanges();
    },

    error: () => {
      this.errorMessage = 'Unable to load categories.';
      this.changeDetectorRef.detectChanges();
    }
  });
}

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadTasks();
  }

  clearFilters(): void {
    this.search = '';
    this.selectedCategoryId = null;
    this.selectedCompletion = '';
    this.pageNumber = 1;

    this.loadTasks();
  }

  previousPage(): void {
    if (this.pageNumber <= 1) {
      return;
    }

    this.pageNumber--;
    this.loadTasks();
  }

  nextPage(): void {
    if (this.pageNumber >= this.totalPages) {
      return;
    }

    this.pageNumber++;
    this.loadTasks();
  }

  deleteTask(task: TodoItem): void {
    const confirmed = window.confirm(
      `Delete task "${task.title}"?`
    );

    if (confirmed === false) {
      return;
    }

    this.errorMessage = '';

    this.todoItemService.delete(task.id).subscribe({
      next: () => {
        if (this.tasks.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }

        this.loadTasks();
      },

      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.errorMessage = 'Task was not found.';
        } else {
          this.errorMessage = 'Unable to delete task.';
        }
      }
    });
  }
}