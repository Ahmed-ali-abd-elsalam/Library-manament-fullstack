// auth.service.ts
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { NotificationService } from './notification-service';

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
  // LocalStorage keys
  private readonly tokenKey = 'access_token';
  private readonly refreshTokenKey = 'refresh_token';
  private readonly userInfoKey = 'user_info';

  private readonly loginUrl = 'https://localhost:7205/api/Auth/login';
  private readonly registerUrl = 'https://localhost:7205/api/Auth/register';
  private readonly forgetPasswordUrl = 'https://localhost:7205/api/Auth/forget-password';
  private readonly forgetPasswordStartUrl = 'https://localhost:7205/api/Auth/forget-Password-start';
  private readonly refreshUrl = 'https://localhost:7205/api/Auth/refresh-token';

  // signals
  private tokenSignal = signal<string | null>(this.getTokenFromStorage());
  private refreshTokenSignal = signal<string | null>(this.getRefreshTokenFromStorage());
  private userInfoSignal = signal<string | null>(this.getUserFromStorage());

  readonly token = this.tokenSignal;
  readonly userInfo = this.userInfoSignal;
  readonly refreshToken = this.refreshTokenSignal;
  readonly isAuthenticated = computed(() => !!this.tokenSignal());

  constructor(private http: HttpClient, private notif: NotificationService) {}

  // role utilities
  isAdmin(): boolean {
    const roles = this.getRolesFromToken();
    return roles.some(
      (r) =>
        /^(admin|administrator)$/i.test(String(r)) ||
        String(r) === '1'
    );
  }

  getRolesFromToken(): string[] {
    const token = this.getToken();
    if (!token) return [];

    try {
      const payload = JSON.parse(atob(token.split('.')[1] || ''));
      const roleClaimKeys = [
        'role',
        'roles',
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role',
        'permissions',
      ];

      for (const key of roleClaimKeys) {
        if (payload[key]) {
          const val = payload[key];
          if (Array.isArray(val)) return val.map(String);
          if (typeof val === 'string') return val.split(',').map((s) => s.trim());
          if (typeof val === 'number') return [String(val)];
        }
      }

      if (payload?.isAdmin === true) return ['Admin'];
    } catch {
      return [];
    }

    return [];
  }

  // local storage helpers
  private getTokenFromStorage() {
    return localStorage.getItem(this.tokenKey);
  }
  private getRefreshTokenFromStorage() {
    return localStorage.getItem(this.refreshTokenKey);
  }
  private getUserFromStorage() {
    return localStorage.getItem(this.userInfoKey);
  }

  getToken(): string | null {
    return this.tokenSignal();
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
    this.tokenSignal.set(token);
  }

  setRefreshToken(token: string): void {
    localStorage.setItem(this.refreshTokenKey, token);
    this.refreshTokenSignal.set(token);
  }

  clearLocalStorage(): void {
    localStorage.removeItem(this.tokenKey);
    this.tokenSignal.set(null);

    localStorage.removeItem(this.userInfoKey);
    this.userInfoSignal.set(null);

    localStorage.removeItem(this.refreshTokenKey);
    this.refreshTokenSignal.set(null);
  }

  setUserInfo(userInfo: UserInfo): void {
    const infoStr = JSON.stringify(userInfo);
    localStorage.setItem(this.userInfoKey, infoStr);
    this.userInfoSignal.set(infoStr);
  }

  // ---------------- LOGIN ----------------
  login(payload: LoginPayload): Observable<void> {
    return this.http
      .post<any>(this.loginUrl, payload, { withCredentials: true })
      .pipe(
        tap((res) => {
          const accessToken = this.extractToken(res.data);
          const refreshToken = res.data?.refresh_token;

          if (accessToken) this.setToken(accessToken);
          if (refreshToken) this.setRefreshToken(refreshToken);

          this.setUserInfo({
            UserName: res.data?.userName || 'User',
            email: res.data?.email || payload.email,
          });

          // LAZY START SIGNALR
          this.notif.startConnection();
        }),
        map(() => void 0)
      );
  }

  register(payload: SignupPayload): Observable<void> {
    return this.http.post<any>(this.registerUrl, payload).pipe(map(() => void 0));
  }

  forgetPasswordStart(email: string): Observable<void> {
    const url = `${this.forgetPasswordStartUrl}?Email=${encodeURIComponent(email)}`;
    return this.http.get<any>(url).pipe(map(() => void 0));
  }

  resetPassword(args: {
    tokenId: string;
    email: string;
    body: { email: string; newPassword: string; confirmNewPassword: string };
  }): Observable<void> {
    const url = `${this.forgetPasswordUrl}?TokenId=${args.tokenId}&Email=${args.email}`;
    return this.http.put<any>(url, args.body).pipe(map(() => void 0));
  }

  refreshAccessToken(): Observable<string | null> {
    const refreshToken = this.refreshTokenSignal();
    if (!refreshToken)
      return new Observable((obs) => obs.next(null));

    return this.http
      .post<any>(this.refreshUrl, { refresh_token: refreshToken }, { withCredentials: true })
      .pipe(
        tap((res) => {
          const newToken = this.extractToken(res.data);
          if (newToken) this.setToken(newToken);
        }),
        map((res) => res?.data?.access_Token || null)
      );
  }

  private extractToken(res: any): string | null {
    if (!res) return null;
    return (
      res.access_Token ||
      res.token ||
      res.jwt ||
      res.id_token ||
      null
    );
  }
}
