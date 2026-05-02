export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export type TaskItemStatus = 'Todo' | 'InProgress' | 'Done';

export interface TaskDto {
  id: string;
  title: string;
  description: string | null;
  status: TaskItemStatus;
  dueDateUtc: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string | null;
  status: TaskItemStatus;
  dueDateUtc: string | null;
}

export interface UpdateTaskRequest {
  title: string;
  description: string | null;
  status: TaskItemStatus;
  dueDateUtc: string | null;
}

export interface CurrentUserResponse {
  userId: string;
  email: string;
}
