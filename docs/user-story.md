# User story (informal)

This document satisfies the interview requirement to drive development from an **informal user story** and to use it during the technical presentation.

---

## Story

**As a** registered user,  
**I want to** sign in securely and manage my personal tasks (create, view, update, delete) with a title, description, status, and optional due date,  
**so that** I can keep track of what I need to do and update progress from any device using the web app.

---

## Context

A small team needs a simple internal task list. Each person should only see and edit **their own** tasks. New users can register; returning users log in. The experience should work on desktop and mobile browsers.

---

## Acceptance criteria (high level)

1. **Authentication**
   - A user can register with email and password.
   - A user can log in and receive a session token (JWT).
   - A user can see who they are when authenticated (e.g. profile / “me”).
   - Unauthenticated users can access only explicitly public endpoints (e.g. health/info).

2. **Tasks**
   - A logged-in user can create a task with at least: title, description (optional), status, due date (optional).
   - A logged-in user can list all of **their** tasks.
   - A logged-in user can open a single task, edit it, or delete it.
   - A user cannot access another user’s tasks (enforced server-side).

3. **Quality**
   - Invalid input is rejected with clear errors.
   - The UI is responsive and supports the full CRUD flow.
   - Demo data and credentials exist so reviewers can try the app without manual setup beyond configuration.

---

## Out of scope (for this exercise)

- Teams, sharing, or role-based admin beyond basic “my tasks”.
- Notifications, recurring tasks, or file attachments.
- Production hardening beyond reasonable defaults (e.g. refresh tokens, rate limiting) unless discussed in review.

---

## Mapping to implementation

| Area | Implementation |
|------|------------------|
| Backend API | ASP.NET Core Web API, Clean Architecture, PostgreSQL, JWT |
| Business rules | Application layer validation and authorization by `userId` |
| Frontend | Angular SPA: login/register, task list and form |
| Demo | Seeded user `demo@local` / `Demo123!` |
