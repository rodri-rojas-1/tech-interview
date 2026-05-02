import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { LoginComponent } from './features/auth/login.component';
import { TaskListComponent } from './features/tasks/task-list.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'tasks',
    component: TaskListComponent,
    canActivate: [authGuard],
  },
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },
  { path: '**', redirectTo: 'tasks' },
];
