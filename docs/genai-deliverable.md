# Generative AI — deliverable (technical interview)

This file supports the **“Generative AI tools”** section: prompt, sample outcome, validation, and corrections.

## 1. Prompt used (summary)

The full one-shot prompt lives at **`AI/prompts/generate-taskmanager-backend.md`**. It instructs the model to act as a senior .NET engineer and to produce:

- ASP.NET Core Web API, Clean Architecture, PostgreSQL, **Npgsql + ADO.NET** (no EF/Dapper/MediatR).
- JWT + BCrypt, task CRUD scoped by user, typed exceptions + middleware mapping.
- xUnit tests and a `Testing` environment that skips DB bootstrap for lightweight integration tests.

## 2. Representative sample of AI output

A typical model response would include:

- `Program.cs` with `AddAuthentication().AddJwtBearer(...)`, `MapControllers()`, and DI extension methods.
- `UserRepository` / `TaskRepository` with `NpgsqlCommand` and `NpgsqlParameter`.
- Controllers returning DTOs and HTTP status codes.

You do **not** need to paste hundreds of lines in the interview; a **short excerpt** (repositories + one controller + one test) is enough if you can walk through it.

## 3. How I validated the AI’s suggestions

| Area | What I checked |
|------|----------------|
| **Constraints** | No EF/Dapper/MediatR; SQL parameterized; ownership by `userId` on every task query. |
| **Security** | Passwords hashed (BCrypt); JWT validation parameters; no secrets in client repo. |
| **C# correctness** | Avoid naming an enum `TaskStatus` (clash with `System.Threading.Tasks.TaskStatus`) — use `TaskItemStatus`. |
| **API contract** | JSON camelCase; enums as strings for the SPA; ProblemDetails-style errors for the frontend. |
| **Tests** | Services testable with mocked `IUserRepository` / `ITaskRepository`; integration smoke without PostgreSQL in `Testing`. |

## 4. What I changed or improved vs raw output

- **Naming**: `TaskItemStatus` instead of `TaskStatus`.
- **Single pipeline**: one exception middleware instead of scattered `try/catch` in controllers.
- **Idempotent startup**: `CREATE TABLE IF NOT EXISTS` + seed only if demo user missing.
- **Frontend alignment**: CORS + Angular proxy + shared enum string values (`Todo`, `InProgress`, `Done`).

## 5. Edge cases, auth, validation

- **Auth**: wrong password → 401 with generic message; duplicate email on register → 409.
- **Tasks**: cross-user access → “not found” (no leak of existence); title/description length limits in application layer.
- **Dates**: optional `dueDateUtc`; client sends ISO; server stores UTC.
- **JWT**: clock skew small; missing/invalid token handled by JWT middleware (401) before controller code runs.

## 6. Optional talking points in the panel

- Why Clean Architecture for a small app (testability, interview signal, clear boundaries).
- Trade-off: raw SQL vs EF (constraint-driven exercise).
- How you would add **migrations** later (Flyway-style SQL scripts or a tiny migrator) without introducing banned packages.
