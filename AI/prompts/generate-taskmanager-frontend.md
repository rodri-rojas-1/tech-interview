# Generate TaskManager frontend (Angular 19)

Act as a senior Angular engineer and generate a production-ready frontend for a Task Manager API.

## Context

- Stack: Angular 19, standalone components, SCSS.
- Backend already exists with JWT auth and these endpoints:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `GET /api/auth/me`
  - `GET/POST /api/tasks`
  - `GET/PUT/DELETE /api/tasks/{taskId}`
- Task status values from API are exact strings: `Todo`, `InProgress`, `Done`.

## Hard constraints

- Do not use NgModules; use standalone components + `app.config.ts`.
- Use Angular `HttpClient` + interceptor for bearer token.
- Keep API base URL environment-driven.
- In development, use proxy with `/api` relative calls.
- Keep code readable and strongly typed.

## Deliverables

1. Create a clean `frontend/src/app` structure with:
   - `core/models` for API DTOs and types
   - `core/services` (`auth.service`, `task.service`)
   - `core/interceptors/auth.interceptor.ts`
   - `core/guards/auth.guard.ts`
   - `features/auth/login.component.*`
   - `features/tasks/task-list.component.*`
2. Implement login/register UI:
   - tabs for login/register
   - form validation
   - persist JWT to localStorage
3. Implement tasks UI:
   - list user tasks
   - create/update/delete tasks
   - mark task as done from list
   - separate completed tasks behind a toggle/history section
4. UX details:
   - status labels in UI should display `To Do`, `In Progress`, `Done` (while preserving API values)
   - mobile-safe input/button font sizes to avoid iOS zoom
   - clean button hierarchy and responsive layout
5. Routing:
   - `/login` public
   - `/tasks` protected by auth guard
   - default route to `/tasks`

## Validation and quality checks

- Include compile-ready code only.
- Ensure interceptor is registered in `app.config.ts`.
- Ensure requests send `Authorization: Bearer <token>` when token exists.
- Avoid circular dependencies in auth/interceptor design.
- Ensure forms trim values before sending when applicable.

## Output format

- First show final file tree.
- Then provide code for each new/modified file.
- Keep comments minimal and meaningful.
