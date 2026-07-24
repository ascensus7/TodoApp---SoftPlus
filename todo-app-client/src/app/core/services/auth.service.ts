import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest
} from '../models/auth.models';
import { TokenStorageService } from './token-storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/auth';

  constructor(
    private http: HttpClient,
    private tokenStorage: TokenStorageService
  ) {
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      this.apiUrl + '/login',
      request
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      this.apiUrl + '/register',
      request
    );
  }

  saveSession(response: AuthResponse): boolean {
    if (
      response.success === false ||
      response.token === null ||
      response.expiration === null
    ) {
      return false;
    }

    this.tokenStorage.saveToken(
      response.token,
      response.expiration
    );

    return true;
  }

  logout(): void {
    this.tokenStorage.removeToken();
  }

  isAuthenticated(): boolean {
    return this.tokenStorage.hasToken();
  }
}