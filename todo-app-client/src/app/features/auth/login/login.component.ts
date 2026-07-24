import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { LoginRequest } from '../../../core/models/auth.models';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  loginForm = new FormGroup({
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

  login(): void {
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const request: LoginRequest = {
      email: this.loginForm.controls.email.value,
      password: this.loginForm.controls.password.value
    };

    this.isLoading = true;

    this.authService.login(request).subscribe({
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
          this.errorMessage = 'Login failed.';
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