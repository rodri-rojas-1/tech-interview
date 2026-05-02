# Claude Code — project context

## What this repo is

Technical interview solution: **TaskManager** — ASP.NET Core Web API, Clean Architecture, PostgreSQL via **Npgsql + ADO.NET** (no EF/Dapper/MediatR), JWT auth, task CRUD, xUnit tests.

Main solution: `TaskManager/TaskManager.slnx`. Angular SPA: **`frontend/`** (`npm start`, proxy to API).

## Architecture and entities (quick)

Backend follows Clean Architecture with four projects:

- `TaskManager.Domain`: core records/enums (`User`, `TaskItem`, `TaskItemStatus`) with no framework dependencies.
- `TaskManager.Application`: use-case services (`AuthService`, `TaskService`), contracts/DTOs, validators, typed exceptions, and repository/security abstractions.
- `TaskManager.Infrastructure`: PostgreSQL/Npgsql implementations (repositories, connection factory), BCrypt/JWT implementations, DB initialization and seeding.
- `TaskManager.API`: HTTP layer (controllers, auth/cors wiring, middleware, startup composition).

Core entities:

- `User`: `Id`, `Email`, `PasswordHash`, `CreatedAtUtc`.
- `TaskItem`: `Id`, `UserId`, `Title`, `Description`, `Status`, `DueDateUtc`, `CreatedAtUtc`, `UpdatedAtUtc`.
- `TaskItemStatus`: `Todo`, `InProgress`, `Done`.

## AI prompts and skills

Reusable prompts and Claude-oriented assets live under **`AI/`** (same level as **`frontend/`**):

- **`AI/prompts/`** — copy into a chat to regenerate or extend parts of the stack.
- **`AI/skills/`** — drop project-specific skills or skill fragments here.

Primary scaffold prompts:

- `AI/prompts/generate-taskmanager-backend.md`
- `AI/prompts/generate-taskmanager-frontend.md`

The human-facing overview stays in **`README.md`** at the repo root (next to this file).

Interview **GenAI** notes (validation, corrections): **`docs/genai-deliverable.md`**.

Project review skills:

- `AI/skills/backend-code-reviewer.md`
- `AI/skills/frontend-code-reviewer.md`

## Conventions

- User-facing and developer exception **messages in English**.
- Demo seed: `demo@local` / `Demo123!` (see README).
- Integration tests use environment **`Testing`** to skip PostgreSQL bootstrap.
- Integration DB resolution order for tests: `appsettings.Testing.json` -> `TASKMANAGER_INTEGRATION_CONNECTION_STRING` -> Docker/Testcontainers.

## Optional local overrides

You may add **`CLAUDE.local.md`** at the repo root (gitignored) for machine-specific notes; do not commit secrets.
