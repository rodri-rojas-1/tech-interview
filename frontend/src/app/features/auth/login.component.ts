import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly mode = signal<'login' | 'register'>('login');
  readonly errorMessage = signal<string | null>(null);
  readonly busy = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      void this.router.navigateByUrl('/tasks');
    }
  }

  setMode(m: 'login' | 'register'): void {
    this.mode.set(m);
    this.errorMessage.set(null);
  }

  submit(): void {
    if (this.form.invalid || this.busy()) {
      this.form.markAllAsTouched();
      return;
    }
    this.busy.set(true);
    this.errorMessage.set(null);
    const body = this.form.getRawValue();
    const req =
      this.mode() === 'login'
        ? this.auth.login(body)
        : this.auth.register(body);
    req.subscribe({
      next: () => void this.router.navigateByUrl('/tasks'),
      error: (err: unknown) => {
        this.busy.set(false);
        this.errorMessage.set(this.formatError(err));
      },
      complete: () => this.busy.set(false),
    });
  }

  private formatError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      const d = err.error as { detail?: string; title?: string } | null;
      if (d?.detail) {
        return d.detail;
      }
      if (typeof err.error === 'string') {
        return err.error;
      }
      return err.message || 'Request failed.';
    }
    return 'Something went wrong.';
  }
}
