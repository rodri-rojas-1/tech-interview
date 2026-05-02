# One-shot prompt: TaskManager backend (Clean Architecture + Web API)

Copy everything below the line into your AI tool as the **user message** (you may prepend repo paths or constraints).

---

You are a **senior .NET engineer** specialized in **ASP.NET Core**, **REST API design**, **Clean Architecture**, **PostgreSQL**, and **testable application design**. You prefer **explicit code over magic**, **small focused types**, and **clear boundaries** between Domain, Application, Infrastructure, and API.

## Goal

Generate a **production-style** backend for a **task management** interview project:

- **.NET** (use the latest stable SDK available in the environment; target the newest LTS or current stable TFM supported by the toolchain).
- **ASP.NET Core Web API** (controllers). Do **not** use Razor MVC views; the UI is a separate SPA.
- **PostgreSQL** as the database.
- **Data access**: **ADO.NET with Npgsql** (`NpgsqlCommand`, parameters). **Do not** use Entity Framework Core, Dapper, or MediatR.
- **Architecture**: Clean Architecture with projects:
  - `Domain` — entities/enums only (no framework references).
  - `Application` — use cases/services, DTOs/contracts, validation, repository **interfaces** (`IUserRepository`, `ITaskRepository`), `IPasswordHasher`, `IJwtTokenGenerator`.
  - `Infrastructure` — Npgsql repositories, `NpgsqlConnectionFactory`, BCrypt hasher, JWT token generator, DB initializer (DDL), demo seeder.
  - `API` — controllers, DI wiring, JWT bearer auth, CORS for `http(s)://localhost:4200`, global exception → HTTP mapping middleware.
- **Auth**:
  - Password hashing with **BCrypt** (via `BCrypt.Net-Next` or equivalent).
  - **JWT** bearer tokens; include claims: `sub` / `NameIdentifier` for user id, email claim(s).
  - Endpoints:
    - `POST /api/auth/register`
    - `POST /api/auth/login`
    - `GET /api/auth/me` (authorized)
  - Public endpoint: `GET /api/public/info` (anonymous).
- **Tasks CRUD** (authorized):
  - `GET/POST /api/tasks`
  - `GET/PUT/DELETE /api/tasks/{taskId}`
  - Tasks belong to the authenticated user; **all queries must filter by `userId`**.
- **Domain model**:
  - `User` record: `Id`, `Email`, `PasswordHash`, `CreatedAtUtc`.
  - `TaskItem` record: `Id`, `UserId`, `Title`, `Description`, `Status`, `DueDateUtc`, `CreatedAtUtc`, `UpdatedAtUtc`.
  - **Enum naming**: use `TaskItemStatus` (values `Todo`, `InProgress`, `Done`) — **avoid** naming an enum `TaskStatus` because it conflicts with `System.Threading.Tasks.TaskStatus` in C#.
- **Validation**: centralize input rules in application layer validators; throw **typed** exceptions (e.g. validation/not-found/conflict/unauthorized-app) mapped to HTTP by middleware.
- **API JSON**: serialize enums as **strings** (`JsonStringEnumConverter`).
- **Startup DB**:
  - On startup (non-testing environment), run `CREATE TABLE IF NOT EXISTS` for `users` and `tasks` with sensible columns/types and an index on `tasks.user_id`.
  - Seed demo user if missing:
    - email `demo@local`, password `Demo123!` (document in README).
    - Insert a couple of sample tasks for that user.
- **Testing environment**:
  - When `ASPNETCORE_ENVIRONMENT=Testing`, **skip** DB initialization/seed so integration tests do not require PostgreSQL.
  - Provide **xUnit** tests:
    - Unit tests for application services with **Moq** (e.g. register conflict, login wrong password, task validation/not found).
    - One **integration smoke** test using `WebApplicationFactory<Program>` against `/api/public/info` with a custom factory that sets environment to `Testing`.
  - Expose `public partial class Program;` for `WebApplicationFactory`.
- **Middleware**: custom middleware (or equivalent) registered early maps application exceptions to ProblemDetails-like JSON (`application/problem+json`) with correct status codes.
- **Messages**: all user-facing exception messages, logs for this layer, and developer exception strings should be **English**.
- **Configuration**: `ConnectionStrings:Default`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` (>= 32 chars for HS256), `Jwt:ExpirationMinutes` in `appsettings.json`.
- **README**: setup steps (create DB, connection string), demo credentials, how to run, how to test, main routes.

## Deliverables

1. Solution structure + project references wired correctly.
2. Working endpoints with correct HTTP verbs and status codes.
3. `dotnet build` and `dotnet test` succeed.
4. `.gitignore` appropriate for .NET + future Node/Angular client.

## Implementation notes

- Keep controllers thin; put rules in Application services.
- Use parameterized SQL only.
- Prefer `IReadOnlyList<T>` for list returns where appropriate.
- Do not add unnecessary packages or scope creep.

Begin by outlining the file/project tree, then implement.

---

_End of prompt._
