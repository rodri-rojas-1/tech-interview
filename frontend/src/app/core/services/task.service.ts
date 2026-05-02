import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import type {
  CreateTaskRequest,
  TaskDto,
  UpdateTaskRequest,
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly http = inject(HttpClient);

  private api(path: string): string {
    const base = environment.apiBaseUrl.replace(/\/$/, '');
    return `${base}${path}`;
  }

  list(): Observable<TaskDto[]> {
    return this.http.get<TaskDto[]>(this.api('/api/tasks'));
  }

  get(id: string): Observable<TaskDto> {
    return this.http.get<TaskDto>(this.api(`/api/tasks/${id}`));
  }

  create(body: CreateTaskRequest): Observable<TaskDto> {
    return this.http.post<TaskDto>(this.api('/api/tasks'), body);
  }

  update(id: string, body: UpdateTaskRequest): Observable<TaskDto> {
    return this.http.put<TaskDto>(this.api(`/api/tasks/${id}`), body);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(this.api(`/api/tasks/${id}`));
  }
}
