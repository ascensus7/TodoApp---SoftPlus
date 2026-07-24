import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';
import { finalize } from 'rxjs';
import { Category } from '../../../core/models/category.models';
import {
  CreateTodoItemRequest,
  UpdateTodoItemRequest
} from '../../../core/models/todo-item.models';
import { CategoryService } from '../../../core/services/category.service';
import { TodoItemService } from '../../../core/services/todo-item.service';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './task-form.component.html'
})
export class TaskFormComponent implements OnInit {
  categories: Category[] = [];

  isEditMode = false;
  isLoading = false;
  isSaving = false;
  errorMessage = '';

  private taskId: number | null = null;

  taskForm = new FormGroup({
    title: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),

    description: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.maxLength(250)
      ]
    }),

    dueDate: new FormControl('', {
      nonNullable: true
    }),

    categoryId: new FormControl<number | null>(null),

    isCompleted: new FormControl(false, {
      nonNullable: true
    })
  });

  constructor(
    private todoItemService: TodoItemService,
    private categoryService: CategoryService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.loadCategories();

    const idParameter =
      this.activatedRoute.snapshot.paramMap.get('id');

    if (idParameter === null) {
      return;
    }

    const parsedTaskId = Number(idParameter);

    if (
      Number.isInteger(parsedTaskId) === false ||
      parsedTaskId <= 0
    ) {
      this.errorMessage = 'Invalid task identifier.';
      return;
    }

    this.taskId = parsedTaskId;
    this.isEditMode = true;

    this.loadTask();
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

  loadTask(): void {
    if (this.taskId === null) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.todoItemService
      .getById(this.taskId)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        })
      )
      .subscribe({
        next: task => {
          this.taskForm.patchValue({
            title: task.title,
            description: task.description ?? '',
            dueDate: this.toDateTimeLocal(task.dueDate),
            categoryId: task.categoryId,
            isCompleted: task.isCompleted
          });
        },

        error: (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.errorMessage = 'Task was not found.';
          } else if (error.status === 401) {
            this.errorMessage =
              'Your session has expired. Please log in again.';
          } else {
            this.errorMessage = 'Unable to load the task.';
          }
        }
      });
  }

  save(): void {
    this.errorMessage = '';

    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    const formValue = this.taskForm.getRawValue();
    const title = formValue.title.trim();

    if (title === '') {
      this.taskForm.controls.title.setErrors({
        required: true
      });

      this.taskForm.controls.title.markAsTouched();
      return;
    }

    const dueDate = formValue.dueDate === ''
      ? null
      : new Date(formValue.dueDate).toISOString();

    this.isSaving = true;

    if (this.isEditMode && this.taskId !== null) {
      const request: UpdateTodoItemRequest = {
        title: title,
        description: this.normalizeText(
          formValue.description
        ),
        isCompleted: formValue.isCompleted,
        dueDate: dueDate,
        categoryId: formValue.categoryId
      };

      this.todoItemService
        .update(this.taskId, request)
        .subscribe({
          next: () => {
            this.router.navigate(['/tasks']);
          },

          error: (error: HttpErrorResponse) => {
            this.handleSaveError(error);
          }
        });

      return;
    }

    const request: CreateTodoItemRequest = {
      title: title,
      description: this.normalizeText(
        formValue.description
      ),
      dueDate: dueDate,
      categoryId: formValue.categoryId
    };

    this.todoItemService.create(request).subscribe({
      next: () => {
        this.router.navigate(['/tasks']);
      },

      error: (error: HttpErrorResponse) => {
        this.handleSaveError(error);
      }
    });
  }

  private normalizeText(value: string): string | null {
    const normalizedValue = value.trim();

    return normalizedValue === ''
      ? null
      : normalizedValue;
  }

  private toDateTimeLocal(value: string | null): string {
    if (value === null) {
      return '';
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return '';
    }

    const timezoneOffset =
      date.getTimezoneOffset() * 60000;

    return new Date(date.getTime() - timezoneOffset)
      .toISOString()
      .slice(0, 16);
  }

  private handleSaveError(
    error: HttpErrorResponse
  ): void {
    this.isSaving = false;

    if (error.status === 400) {
      this.errorMessage = 'Check the entered data.';
    } else if (error.status === 404) {
      this.errorMessage =
        'The task or category was not found.';
    } else {
      this.errorMessage = 'Unable to save the task.';
    }
  }
}