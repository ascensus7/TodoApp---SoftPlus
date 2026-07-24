import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { RegisterRequest } from '../../../core/models/auth.models';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  registerForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.email
      ]
    }),

    password: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(6)
      ]
    }),

    confirmPassword: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required
      ]
    })
  });

  errorMessage = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
  }

  register(): void {
    this.errorMessage = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const password =
      this.registerForm.controls.password.value;

    const confirmPassword =
      this.registerForm.controls.confirmPassword.value;

    if (password !== confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    const request: RegisterRequest = {
      email: this.registerForm.controls.email.value,
      password: password
    };

    this.isLoading = true;

    this.authService.register(request).subscribe({
      next: response => {
        this.isLoading = false;

        const sessionSaved =
          this.authService.saveSession(response);

        if (sessionSaved) {
          this.router.navigate(['/tasks']);
          return;
        }

        if (response.errors.length > 0) {
          this.errorMessage = response.errors.join(' ');
        } else {
          this.errorMessage = 'Registration failed.';
        }
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading = false;

        if (
          error.error !== null &&
          error.error !== undefined &&
          Array.isArray(error.error.errors)
        ) {
          this.errorMessage = error.error.errors.join(' ');
        } else {
          this.errorMessage =
            'Unable to connect to the server.';
        }
      }
    });
  }
}