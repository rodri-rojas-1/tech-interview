# TaskManager (technical interview)

Backend **ASP.NET Core Web API** with **Clean Architecture**, data access via **Npgsql + ADO.NET** (no EF/Dapper/MediatR), **JWT** authentication, task CRUD, and demo seed data. **Angular** SPA under **`frontend/`** (login/register, task CRUD).

**Claude Code** context: see **`CLAUDE.md`** at this same level. **AI prompts/skills**: **`AI/prompts/`**, **`AI/skills/`**. **GenAI interview deliverable** (validation, corrections, talking points): **`docs/genai-deliverable.md`**. **Informal user story** (for the presentation): **`docs/user-story.md`**.

## Repository layout

```text
tech-interview/
├── README.md                 # this file
├── CLAUDE.md                 # Claude Code project instructions
├── frontend/                 # Angular SPA (npm start)
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
- [Node.js](https://nodejs.org/) LTS (for `frontend/`)

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

- **Unit tests** cover application services, validators, and **API controllers** (mocked dependencies).
- **Integration (no DB)**: `GET /api/public/info` with environment `Testing` (no PostgreSQL).
- **Integration (PostgreSQL)** — repository and HTTP CRUD tests run when **either**:
  - **Docker** is running (Testcontainers starts PostgreSQL), or  
  - you set **`TASKMANAGER_INTEGRATION_CONNECTION_STRING`** to a real Postgres connection string (e.g. a dedicated `taskmanager_test` database).

If neither is available, those tests are **skipped** (not failed) so `dotnet test` still passes locally/CI.

## Run the frontend

```bash
cd frontend
npm install
npm start
```

Uses `http://localhost:4200` and proxies `/api` to the backend (see `frontend/proxy.conf.json`). Start the API first; default proxy target is **`http://localhost:5256`**. Details: **`frontend/README.md`**.

## Full stack (typical dev)

1. Terminal A — PostgreSQL + `cd TaskManager && dotnet run --project TaskManager.API/...`  
2. Terminal B — `cd frontend && npm start`  
3. Browser — `http://localhost:4200` → login with seed user `demo@local` / `Demo123!` (or register a new user).
