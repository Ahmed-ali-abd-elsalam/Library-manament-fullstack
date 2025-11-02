import { Injectable, signal } from '@angular/core';
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

export interface UserInfo {
  UserName: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenSignal = signal<string | null>(this.getTokenFromStorage());
  private userInfoSignal = signal<string | null>(this.getUserFromStorage());
  private readonly loginUrl = 'https://localhost:7205/api/Auth/login';
  private readonly registerUrl = 'https://localhost:7205/api/Auth/register';
  private readonly tokenKey = 'access_token';
  private readonly userInfoKey = 'user_info';

  constructor(private http: HttpClient) {}

  private getTokenFromStorage() {
    return localStorage.getItem(this.tokenKey);
  }
  private getUserFromStorage() {
    return localStorage.getItem(this.userInfoKey);
  }
  getToken(): string | null {
    try {
      return this.tokenSignal();
    } catch {
      return null;
    }
  }

  setToken(token: string): void {
    try {
      localStorage.setItem(this.tokenKey, token);
      this.tokenSignal.set(token);
    } catch {}
  }

  clearToken(): void {
    try {
      localStorage.removeItem(this.tokenKey);
      this.tokenSignal.set(null);
      localStorage.removeItem(this.userInfoKey);
    } catch {}
  }

  getUserInfo(): string | null {
    try {
      const userInfoStr = localStorage.getItem(this.userInfoKey);
      return this.userInfoSignal();
    } catch {
      return null;
    }
  }

  setUserInfo(userInfo: UserInfo): void {
    try {
      localStorage.setItem(this.userInfoKey, JSON.stringify(userInfo));
      this.userInfoSignal.set(JSON.stringify(userInfo));
    } catch {}
  }

  login(payload: LoginPayload): Observable<void> {
    return this.http.post<any>(this.loginUrl, payload).pipe(
      tap((res) => {
        const token = this.extractToken(res.data);
        if (token) {
          this.setToken(token);
        }

        if (res.data?.name || res.data?.email) {
          this.setUserInfo({
            UserName: res.data.UserName || 'User',
            email: res.data.email || payload.email,
          });
        }
      }),
      map(() => void 0)
    );
  }

  register(payload: SignupPayload): Observable<void> {
    return this.http.post<any>(this.registerUrl, payload).pipe(map(() => void 0));
  }

  private extractToken(res: any): string | null {
    if (!res || typeof res !== 'object') return null;
    return res.access_Token || res.token || res.jwt || res.id_token || null;
  }
}
