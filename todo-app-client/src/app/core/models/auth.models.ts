export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  userId: string | null;
  email: string | null;
  token: string | null;
  expiration: string | null;
  errors: string[];
}