# Generate TaskManager backend (Clean Architecture + Web API)

Act as a senior .NET engineer and generate a production-ready backend for a Task Manager interview project.

## Context

- Stack: .NET 10 (or latest stable available), ASP.NET Core Web API, PostgreSQL.
- Architecture: Clean Architecture (`Domain`, `Application`, `Infrastructure`, `API`).
- Frontend is a separate SPA, so backend must expose JSON-only REST endpoints.

## Hard constraints

- Use controllers (`[ApiController]` + routes).
- Data access must be **Npgsql + ADO.NET only** (`NpgsqlCommand`, parameters).
- Do not use Entity Framework Core, Dapper, or MediatR.
- Keep all user-facing messages in English.
- Use `TaskItemStatus` enum (`Todo`, `InProgress`, `Done`) to avoid `TaskStatus` naming conflict.

## Functional requirements

1. Auth (JWT + BCrypt):
   - `POST /api/auth/register`
   - `POST /api/auth/login`
   - `GET /api/auth/me` (authorized)
2. Public endpoint:
   - `GET /api/public/info`
3. Task CRUD (authorized):
   - `GET /api/tasks`
   - `POST /api/tasks`
   - `GET /api/tasks/{taskId}`
   - `PUT /api/tasks/{taskId}`
   - `DELETE /api/tasks/{taskId}`
4. Tasks must always be filtered/scoped by authenticated `userId`.

## Technical requirements

- `Domain`: records/enums only (no framework dependencies).
- `Application`: services/use-cases, DTO/contracts, validators, abstractions (`IUserRepository`, `ITaskRepository`, `IPasswordHasher`, `IJwtTokenGenerator`), typed exceptions.
- `Infrastructure`: repository implementations, connection factory, BCrypt hasher, JWT generator, DB initializer, demo seeder.
- `API`: controllers, DI wiring, JWT bearer setup, CORS for `http://localhost:4200` and `https://localhost:4200`, exception middleware mapping typed exceptions to `application/problem+json`.
- JSON enums must be serialized as strings (`JsonStringEnumConverter`).

## Startup and seed behavior

- On startup (non-testing), run `CREATE TABLE IF NOT EXISTS` for `users` and `tasks`.
- Add index on `tasks.user_id`.
- Seed demo user if missing:
  - `demo@local` / `Demo123!`
  - include sample tasks
- If environment is `Testing`, skip DB initialization/seed.

## Tests requirements

- Use xUnit.
- Add unit tests with Moq for core `Application` services and selected controllers.
- Add integration smoke test with `WebApplicationFactory<Program>` for `/api/public/info`.
- Expose `public partial class Program;` for test host usage.

## Deliverables

1. Full file tree first.
2. Then code for each file needed for a runnable solution.
3. `dotnet build` and `dotnet test` should pass.
4. Include README setup notes (DB, run, test, demo credentials).

## Output format

- Start with solution/project tree.
- Then provide implementation by project (`Domain`, `Application`, `Infrastructure`, `API`, tests).
- Keep comments short and meaningful.
