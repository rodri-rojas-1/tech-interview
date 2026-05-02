# Claude Code — project context

## What this repo is

Technical interview solution: **TaskManager** — ASP.NET Core Web API, Clean Architecture, PostgreSQL via **Npgsql + ADO.NET** (no EF/Dapper/MediatR), JWT auth, task CRUD, xUnit tests.

Main solution: `TaskManager/TaskManager.slnx`. SPA will live under **`frontend/`**.

## AI prompts and skills

Reusable prompts and Claude-oriented assets live under **`AI/`** (same level as **`frontend/`**):

- **`AI/prompts/`** — copy into a chat to regenerate or extend parts of the stack.
- **`AI/skills/`** — drop project-specific skills or skill fragments here.

Primary backend scaffold prompt:

- `AI/prompts/generate-taskmanager-backend.md`

The human-facing overview stays in **`README.md`** at the repo root (next to this file).

## Conventions

- User-facing and developer exception **messages in English**.
- Demo seed: `demo@local` / `Demo123!` (see README).
- Integration tests use environment **`Testing`** to skip PostgreSQL bootstrap.

## Optional local overrides

You may add **`CLAUDE.local.md`** at the repo root (gitignored) for machine-specific notes; do not commit secrets.
