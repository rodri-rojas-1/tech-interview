import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AUTH_TOKEN_KEY, readStoredToken } from '../constants/auth-storage';
import type {
  AuthResponse,
  CurrentUserResponse,
  LoginRequest,
} from '../models/api.models';

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
      tap((r) => this.persistToken(r.token)),
    );
  }

  register(body: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.api('/api/auth/register'), body).pipe(
      tap((r) => this.persistToken(r.token)),
    );
  }

  me(): Observable<CurrentUserResponse> {
    return this.http.get<CurrentUserResponse>(this.api('/api/auth/me'));
  }

  logout(): void {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    this.token.set(null);
    void this.router.navigateByUrl('/login');
  }

  private persistToken(t: string): void {
    localStorage.setItem(AUTH_TOKEN_KEY, t);
    this.token.set(t);
  }
}
