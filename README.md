# TaskManager (technical interview)

Backend **ASP.NET Core Web API** with **Clean Architecture**, data access via **Npgsql + ADO.NET** (no EF/Dapper/MediatR), **JWT** authentication, task CRUD, and demo seed data. SPA will live under **`frontend/`**.

**Claude Code** context: see **`CLAUDE.md`** at this same level. **AI prompts/skills**: **`AI/prompts/`**, **`AI/skills/`**.

## Repository layout

```text
tech-interview/
├── README.md                 # this file
├── CLAUDE.md                 # Claude Code project instructions
├── frontend/                 # SPA (Angular) — scaffold when ready
├── AI/
│   ├── README.md
│   ├── prompts/              # reusable generation prompts
│   └── skills/               # Claude skills / snippets
└── TaskManager/              # .NET solution
    └── TaskManager.slnx
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Local [PostgreSQL](https://www.postgresql.org/) (default port `5432`)

## Database

1. Create an empty database, e.g. `taskmanager`:

   ```sql
   CREATE DATABASE taskmanager;
   ```

2. Set `ConnectionStrings:Default` in `TaskManager/TaskManager.API/appsettings.json` to match your PostgreSQL user/password.

On startup (any environment other than `Testing`), the API creates the `users` and `tasks` tables and runs the seed.

## Demo credentials (seed)

| Field    | Value        |
|----------|--------------|
| Email    | `demo@local` |
| Password | `Demo123!`   |

## Run the API

```bash
cd TaskManager
dotnet run --project TaskManager.API/TaskManager.API.csproj
```

HTTPS dev URL is printed in the console. CORS allows `http(s)://localhost:4200` for the **`frontend/`** app.

## Main endpoints

| Method | Route | Auth |
|--------|------|------|
| GET | `/api/public/info` | No |
| POST | `/api/auth/register` | No |
| POST | `/api/auth/login` | No |
| GET | `/api/auth/me` | Yes (Bearer) |
| GET/POST | `/api/tasks` | Yes |
| GET/PUT/DELETE | `/api/tasks/{taskId}` | Yes |

`TaskItemStatus` is serialized as a JSON string: `Todo`, `InProgress`, `Done`.

## Tests

```bash
cd TaskManager
dotnet test TaskManager.slnx
```

Integration tests use the `Testing` environment and **do not** initialize PostgreSQL (smoke test for the public endpoint). Unit tests cover application rules with mocked dependencies.

## Next step

Add the Angular app under **`frontend/`** (see `frontend/README.md`): login, list, and task CRUD against this API.
