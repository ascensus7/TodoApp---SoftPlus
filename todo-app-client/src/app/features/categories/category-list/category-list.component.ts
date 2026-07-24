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
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest
} from '../../../core/models/category.models';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './category-list.component.html'
})
export class CategoryListComponent implements OnInit {
  categories: Category[] = [];

  editingCategoryId: number | null = null;

  isLoading = false;
  isSaving = false;
  errorMessage = '';

  categoryForm = new FormGroup({
    name: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    })
  });

  constructor(
    private categoryService: CategoryService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.categoryService.getAll().subscribe({
      next: categories => {
        this.categories = categories;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },

      error: () => {
        this.isLoading = false;
        this.errorMessage = 'Unable to load categories.';
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  save(): void {
    this.errorMessage = '';

    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const name = this.categoryForm.controls.name.value.trim();

    if (name === '') {
      this.categoryForm.controls.name.setErrors({
        required: true
      });

      this.categoryForm.controls.name.markAsTouched();
      return;
    }

    this.isSaving = true;

    if (this.editingCategoryId !== null) {
      const request: UpdateCategoryRequest = {
        name: name
      };

      this.categoryService
        .update(this.editingCategoryId, request)
        .subscribe({
          next: updatedCategory => {
            const index = this.categories.findIndex(
              category => category.id === updatedCategory.id
            );

            if (index !== -1) {
              this.categories[index] = updatedCategory;

              this.categories = [...this.categories].sort(
                (first, second) =>
                  first.name.localeCompare(second.name)
              );
            }

            this.resetForm();
            this.changeDetectorRef.detectChanges();
          },

          error: (error: HttpErrorResponse) => {
            this.handleSaveError(error);
            this.changeDetectorRef.detectChanges();
          }
        });

      return;
    }

    const request: CreateCategoryRequest = {
      name: name
    };

    this.categoryService.create(request).subscribe({
      next: createdCategory => {
        this.categories = [
          ...this.categories,
          createdCategory
        ].sort(
          (first, second) =>
            first.name.localeCompare(second.name)
        );

        this.resetForm();
        this.changeDetectorRef.detectChanges();
      },

      error: (error: HttpErrorResponse) => {
        this.handleSaveError(error);
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  startEditing(category: Category): void {
    this.editingCategoryId = category.id;
    this.errorMessage = '';

    this.categoryForm.setValue({
      name: category.name
    });
  }

  cancelEditing(): void {
    this.resetForm();
  }

  deleteCategory(category: Category): void {
    const confirmed = window.confirm(
      `Delete category "${category.name}"? Tasks in it will remain without a category.`
    );

    if (confirmed === false) {
      return;
    }

    this.errorMessage = '';

    this.categoryService.delete(category.id).subscribe({
      next: () => {
        this.categories = this.categories.filter(
          currentCategory =>
            currentCategory.id !== category.id
        );

        if (this.editingCategoryId === category.id) {
          this.resetForm();
        }

        this.changeDetectorRef.detectChanges();
      },

      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.errorMessage = 'Category was not found.';
        } else {
          this.errorMessage = 'Unable to delete category.';
        }

        this.changeDetectorRef.detectChanges();
      }
    });
  }

  private resetForm(): void {
    this.editingCategoryId = null;
    this.isSaving = false;
    this.categoryForm.reset();
  }

  private handleSaveError(
    error: HttpErrorResponse
  ): void {
    this.isSaving = false;

    if (error.status === 409) {
      this.errorMessage =
        'A category with the same name already exists.';
    } else if (error.status === 400) {
      this.errorMessage = 'Check the category name.';
    } else if (error.status === 404) {
      this.errorMessage = 'Category was not found.';
    } else {
      this.errorMessage = 'Unable to save category.';
    }
  }
}