# Generative AI — deliverable

This file supports the **“Generative AI tools”** section: prompt, sample outcome, validation, and corrections.

## 1. Prompt used (summary)

The full one-shot backend prompt lives at **`AI/prompts/generate-taskmanager-backend.md`**.  
The frontend counterpart prompt now lives at **`AI/prompts/generate-taskmanager-frontend.md`**.

The backend prompt instructs the model to act as a senior .NET engineer and to produce:

- ASP.NET Core Web API, Clean Architecture, PostgreSQL, **Npgsql + ADO.NET** (no EF/Dapper/MediatR).
- JWT + BCrypt, task CRUD scoped by user, typed exceptions + middleware mapping.
- xUnit tests and a `Testing` environment that skips DB bootstrap for lightweight integration tests.
- Frontend is intentionally prompted separately to keep concerns clear (`generate-taskmanager-frontend.md`).

## 2. Representative sample of AI output

Below are representative excerpts from the generated implementation.

**A) API controller sample (`TasksController`)**

```csharp
[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> List(CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        var result = await _tasks.ListAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        var created = await _tasks.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { taskId = created.Id }, created);
    }
}
```

Why representative: shows REST shape, auth requirement, and user-scoped task behavior.

**B) Repository SQL sample (`TaskRepository`)**

```csharp
await using var command = new NpgsqlCommand(
    """
    SELECT id, user_id, title, description, status, due_date_utc, created_at_utc, updated_at_utc
    FROM tasks
    WHERE user_id = @user_id
    ORDER BY updated_at_utc DESC
    """,
    connection);

command.Parameters.Add(new NpgsqlParameter("user_id", userId));
```

Why representative: confirms parameterized SQL and no EF/Dapper usage.

**C) Frontend auth integration sample (`auth.interceptor.ts`)**

```ts
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = readStoredToken();
  if (!token) {
    return next(req);
  }
  const headers = req.headers.set('Authorization', `Bearer ${token}`);
  return next(req.clone({ headers }));
};
```

Why representative: shows how the SPA attaches JWT bearer tokens to protected API calls.

## 3. How I validated the AI’s suggestions

- I like to debug endpoint by endpoint to verify behavior, code quality, and readability.
- While debugging, I also check PostgreSQL directly to confirm values are persisted/updated as expected.
- I validated constraints: no EF/Dapper/MediatR, SQL is parameterized, and task queries are scoped by `userId`.
- I validated security and contracts: BCrypt hashing, JWT validation, JSON camelCase, enum strings for SPA.
- I used ngrok to test the app from mobile and detect UI/runtime issues in real device-like usage.
- Before committing, I run a Claude skill-based code review pass to catch quality issues.
- I reviewed C# correctness details, including avoiding `TaskStatus` naming conflicts by using `TaskItemStatus`.
- I reviewed the API contract details: JSON camelCase, enum-as-string behavior for SPA compatibility, and ProblemDetails-style error payloads.
- I validated how DB seed is implemented (idempotent startup, demo user creation only if missing, sample tasks insertion).

## 4. What I changed or improved vs raw output

- **Naming**: `TaskItemStatus` instead of `TaskStatus`.
- **Single pipeline**: one exception middleware instead of scattered `try/catch` in controllers.
- **Idempotent startup**: `CREATE TABLE IF NOT EXISTS` + seed only if demo user missing.
- **CORS**: explicit allowlist for `http(s)://localhost:4200`.
- **Angular proxy**: adjusted dev proxy behavior to avoid auth header loss on redirect scenarios.
- **Shared enum values**: aligned backend/frontend status values (`Todo`, `InProgress`, `Done`) and added UI-friendly labels.
- **Ngrok/mobile checks**: added remote mobile verification flow to catch issues that desktop-only testing misses.
- **iPhone zoom bug**: fixed iOS auto-zoom on form focus by adjusting mobile-safe font sizing.
- **Tests**: added more tests to increase coverage and cover additional error branches.
- **Mobile UX**: improved button sizing/layout and interaction patterns for touch devices.
- **Done history**: added a completed-tasks history section because hiding done tasks entirely did not make functional sense.
- **Additional bugs**: fixed several other small bugs.
- **Additional UI tweaks**: applied several other small UI tweaks.

## 5. Edge cases, auth, validation

- **Auth**: wrong password → 401 with generic message; duplicate email on register → 409.
- **Tasks**: cross-user access → “not found” (no leak of existence); title/description length limits in application layer.
- **Dates**: optional `dueDateUtc`; client sends ISO; server stores UTC.
