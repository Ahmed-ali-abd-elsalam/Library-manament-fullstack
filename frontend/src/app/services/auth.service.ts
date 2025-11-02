import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';

export interface LoginPayload {
  email: string;
  password: string;
}

export interface SignupPayload {
  name: string;
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly loginUrl = 'https://localhost:7205/api/Auth/login';
  private readonly registerUrl = 'https://localhost:7205/api/Auth/register';
  private readonly tokenKey = 'access_token';

  constructor(private http: HttpClient) {}

  getToken(): string | null {
    try {
      return localStorage.getItem(this.tokenKey);
    } catch {
      return null;
    }
  }

  setToken(token: string): void {
    try {
      localStorage.setItem(this.tokenKey, token);
    } catch {}
  }

  clearToken(): void {
    try {
      localStorage.removeItem(this.tokenKey);
    } catch {}
  }

  login(payload: LoginPayload): Observable<void> {
    return this.http.post<any>(this.loginUrl, payload).pipe(
      tap((res) => {

        const token = this.extractToken(res.data);
        if (token) {
          this.setToken(token);
        }
      }),
      map(() => void 0)
    );
  }

  register(payload: SignupPayload): Observable<void> {
    return this.http.post<any>(this.registerUrl, payload).pipe(map(() => void 0));
  }

  private extractToken(res: any): string | null {
    console.log(res);
    if (!res || typeof res !== 'object') return null;
    return (
      res.access_Token ||
      res.token ||
      res.jwt ||
      res.id_token ||
      null
    );
  }
}
