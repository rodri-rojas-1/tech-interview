import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AUTH_TOKEN_KEY, readStoredToken } from '../constants/auth-storage';
import { bearerAuthOptions } from '../http/bearer-options';
import type {
  AuthResponse,
  CurrentUserResponse,
  LoginRequest,
} from '../models/api.models';

function pickAccessToken(response: AuthResponse & { Token?: string }): string {
  const raw = response.token ?? response.Token;
  return typeof raw === 'string' ? raw.trim() : '';
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly token = signal<string | null>(readStoredToken());

  readonly isAuthenticated = computed(() => !!this.token());

  private api(path: string): string {
    const base = environment.apiBaseUrl.replace(/\/$/, '');
    return `${base}${path}`;
  }

  login(body: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.api('/api/auth/login'), body).pipe(
      tap((r) => {
        const access = pickAccessToken(r);
        if (access) {
          this.persistToken(access);
        }
      }),
    );
  }

  register(body: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.api('/api/auth/register'), body).pipe(
      tap((r) => {
        const access = pickAccessToken(r);
        if (access) {
          this.persistToken(access);
        }
      }),
    );
  }

  me(): Observable<CurrentUserResponse> {
    return this.http.get<CurrentUserResponse>(this.api('/api/auth/me'), {
      ...bearerAuthOptions(),
    });
  }

  logout(): void {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    this.token.set(null);
    void this.router.navigateByUrl('/login');
  }

  private persistToken(t: string): void {
    const v = t.trim();
    if (!v) {
      return;
    }
    localStorage.setItem(AUTH_TOKEN_KEY, v);
    this.token.set(v);
  }
}
