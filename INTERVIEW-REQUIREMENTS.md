# INTERVIEW REQUIREMENTS

This document maps requested interview requirements to how they were interpreted and implemented in this repository.

## Requirement checklist

| Requirement requested | Implementation in this repo | Where to verify |
|---|---|---|
| Clean Architecture | Split by concerns (`API`, `Application`, `Domain`, `Infrastructure`) | `TaskManager/` solution structure |
| TDD | Test-first workflow applied in core services/controllers and validated with unit/integration suites | `TaskManager.Tests.Unit`, `TaskManager.Tests.Integration` |
| Data layer | Implemented in `Infrastructure` with repositories and DB bootstrap/seed | `TaskManager.Infrastructure/Repositories`, `TaskManager.Infrastructure/Database` |
| Business logic layer | Implemented in `Application` services + validation, with domain rules/types in `Domain` | `TaskManager.Application/Services`, `TaskManager.Application/Validation`, `TaskManager.Domain` |
| REST API with auth | ASP.NET Core controllers + JWT bearer authentication | `TaskManager.API/Controllers`, `TaskManager.API/Program.cs` |
| Task CRUD per user | CREATE, GET ALL, GET BY ID, UPDATE, DELETE | `TasksController`, `TaskService`, repositories |
| Storage | PostgreSQL via Npgsql + ADO.NET | `TaskManager.Infrastructure` |
| Frontend | Angular 19 SPA | `frontend/src/app` |
| Security | Password hashing (BCrypt) + JWT token issuance/validation | `TaskManager.Infrastructure/Security`, `TaskManager.API/Program.cs` |
| Tests | xUnit unit tests + integration tests (public API, DB/repository, authenticated HTTP flow) | `TaskManager.Tests.Unit`, `TaskManager.Tests.Integration` |
| Code coverage | +50% considering unit + integration test execution | `TaskManager.Tests.Unit`, `TaskManager.Tests.Integration`, coverage reports |
| Demo readiness | Seed user + local run instructions for backend/frontend | `README.md`, `TaskManager.Infrastructure/Database/DemoDataSeeder.cs` |
| User story | Own Markdown file | **`docs/user-story.md`** |
| GenAI deliverable | Own Markdown file with validation/corrections/talking points | **`docs/genai-deliverable.md`** |
| Other: No EF / Dapper / MediatR | Data access implemented with Npgsql + ADO.NET only | `TaskManager.Infrastructure`, `TaskManager/README.md` |
| Other: No console warnings | Frontend/build adjusted to run clean without warnings in normal flow | `frontend/`, `README.md` |
| Other: Code quality | Clean structure, validation, and exception middleware | `TaskManager.Application`, `TaskManager.API/Middleware` |

## Notes for interviewer

- Status values are API-facing (`Todo`, `InProgress`, `Done`) and user-facing labels in UI are formatted (`To Do`, `In Progress`, `Done`). Not the best approach, but it's a trade-off between simplicity and flexibility.
- Integration tests that require PostgreSQL resolve DB in this order: `appsettings.Testing.json`, `TASKMANAGER_INTEGRATION_CONNECTION_STRING`, Docker/Testcontainers; if none is available, they are skipped by design. This approach could be confusing but I wanted to be as flexible as possible.
