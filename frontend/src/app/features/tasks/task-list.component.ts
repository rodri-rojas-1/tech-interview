import { CommonModule, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import type {
  TaskDto,
  TaskItemStatus,
} from '../../core/models/api.models';
import { TaskStatusLabelPipe } from '../../core/pipes/task-status-label.pipe';
import { AuthService } from '../../core/services/auth.service';
import { TaskService } from '../../core/services/task.service';

const STATUSES: TaskItemStatus[] = ['Todo', 'InProgress', 'Done'];

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePipe, TaskStatusLabelPipe],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss',
})
export class TaskListComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly tasksApi = inject(TaskService);
  private readonly auth = inject(AuthService);

  readonly userEmail = signal<string | null>(null);
  readonly tasks = signal<TaskDto[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly busy = signal(false);
  readonly editingId = signal<string | null>(null);
  readonly formExpanded = signal(false);
  readonly showHistory = signal(false);

  readonly statuses = STATUSES;
  readonly activeTasks = computed(() =>
    this.tasks().filter((task) => task.status !== 'Done'),
  );
  readonly completedTasks = computed(() =>
    this.tasks().filter((task) => task.status === 'Done'),
  );

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(500)]],
    description: ['', [Validators.maxLength(5000)]],
    status: ['Todo' as TaskItemStatus, Validators.required],
    dueLocal: [''],
  });

  ngOnInit(): void {
    this.auth.me().subscribe({
      next: (u) => this.userEmail.set(u.email),
      error: () => this.userEmail.set(null),
    });
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.error.set(null);
    this.tasksApi.list().subscribe({
      next: (list) => {
        this.tasks.set(list);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.loading.set(false);
        this.error.set(this.formatHttpError(err));
      },
    });
  }

  startCreate(): void {
    this.formExpanded.set(true);
    this.editingId.set(null);
    this.form.reset({
      title: '',
      description: '',
      status: 'Todo',
      dueLocal: '',
    });
  }

  startEdit(t: TaskDto): void {
    this.formExpanded.set(true);
    this.editingId.set(t.id);
    this.form.setValue({
      title: t.title,
      description: t.description ?? '',
      status: t.status,
      dueLocal: toLocalDatetimeInput(t.dueDateUtc),
    });
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.formExpanded.set(false);
    this.form.reset({
      title: '',
      description: '',
      status: 'Todo',
      dueLocal: '',
    });
  }

  toggleCreatePanel(): void {
    if (this.formExpanded()) {
      this.cancelEdit();
      return;
    }
    this.startCreate();
  }

  toggleHistory(): void {
    this.showHistory.update((value) => !value);
  }

  save(): void {
    if (this.form.invalid || this.busy()) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const dueDateUtc = v.dueLocal
      ? new Date(v.dueLocal).toISOString()
      : null;
    const id = this.editingId();
    this.busy.set(true);
    this.error.set(null);

    if (id) {
      this.tasksApi
        .update(id, {
          title: v.title.trim(),
          description: v.description.trim() || null,
          status: v.status,
          dueDateUtc,
        })
        .subscribe({
          next: () => {
            this.busy.set(false);
            this.cancelEdit();
            this.refresh();
          },
          error: (err: unknown) => {
            this.busy.set(false);
            this.error.set(this.formatHttpError(err));
          },
        });
    } else {
      this.tasksApi
        .create({
          title: v.title.trim(),
          description: v.description.trim() || null,
          status: v.status,
          dueDateUtc,
        })
        .subscribe({
          next: () => {
            this.busy.set(false);
            this.startCreate();
            this.refresh();
          },
          error: (err: unknown) => {
            this.busy.set(false);
            this.error.set(this.formatHttpError(err));
          },
        });
    }
  }

  remove(t: TaskDto): void {
    if (!confirm(`Delete "${t.title}"?`)) {
      return;
    }
    this.busy.set(true);
    this.error.set(null);
    this.tasksApi.delete(t.id).subscribe({
      next: () => {
        this.busy.set(false);
        if (this.editingId() === t.id) {
          this.cancelEdit();
        }
        this.refresh();
      },
      error: (err: unknown) => {
        this.busy.set(false);
        this.error.set(this.formatHttpError(err));
      },
    });
  }

  markDone(t: TaskDto): void {
    if (t.status === 'Done' || this.busy()) {
      return;
    }
    this.busy.set(true);
    this.error.set(null);
    this.tasksApi
      .update(t.id, {
        title: t.title,
        description: t.description,
        status: 'Done',
        dueDateUtc: t.dueDateUtc,
      })
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.refresh();
        },
        error: (err: unknown) => {
          this.busy.set(false);
          this.error.set(this.formatHttpError(err));
        },
      });
  }

  logout(): void {
    this.auth.logout();
  }

  private formatHttpError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      const d = err.error as { detail?: string } | null;
      if (d?.detail) {
        return d.detail;
      }
      return err.message || 'Request failed.';
    }
    return 'Something went wrong.';
  }
}

function toLocalDatetimeInput(isoUtc: string | null): string {
  if (!isoUtc) {
    return '';
  }
  const d = new Date(isoUtc);
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}
