import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {
  private tokenKey = 'todo_app_token';
  private expirationKey = 'todo_app_token_expiration';

  saveToken(token: string, expiration: string): void {
    localStorage.setItem(this.tokenKey, token);
    localStorage.setItem(this.expirationKey, expiration);
  }

  getToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);
    const expiration = localStorage.getItem(this.expirationKey);

    if (token === null || expiration === null) {
      return null;
    }

    const expirationTime = new Date(expiration).getTime();

    if (isNaN(expirationTime) || expirationTime <= Date.now()) {
      this.removeToken();
      return null;
    }

    return token;
  }

  hasToken(): boolean {
    return this.getToken() !== null;
  }

  removeToken(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.expirationKey);
  }
}