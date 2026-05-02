# Backend Review Skill

Use this skill to review backend code changes in this repository (API, Application, Domain, Infrastructure, tests).

## Scope

- ASP.NET Core API behavior and HTTP contracts
- Business logic and validation rules
- Data access safety and SQL parameterization
- Auth/security basics (JWT, claims, permissions)
- Test quality and risk of regressions

## Review approach

1. Prioritize correctness and regression risk over style preferences.
2. Flag only meaningful issues (bugs, security risks, broken behavior, missing critical tests).
3. Keep feedback actionable and minimal.
4. Suggest code changes only when they materially improve correctness, safety, or maintainability.

## Non-forcing rule (important)

- If the current implementation is already acceptable, do **not** force changes just to produce output.
- It is valid to conclude: "No changes needed."
- Avoid "cosmetic-only" refactors unless explicitly requested by the user.

## Output format

- Findings first (highest severity first)
- Then optional improvements
- Then short final verdict: `changes required` or `no changes needed`
