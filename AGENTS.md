# FlowState Planner Agent Guidelines

Scope: entire repository.

## Architecture and layering
- Keep this repository as a modular monolith.
- Respect project boundaries:
  - `Domain`: entities, value objects, enums, and domain rules only.
  - `Application`: use-cases, DTOs, interfaces, validation, and orchestration logic.
  - `Infrastructure`: EF Core, external integrations, persistence, and implementation details.
  - `Api`: HTTP transport concerns only.
- Domain must not depend on Application, Infrastructure, or Api.

## Coding standards
- Target .NET 8 and nullable reference types.
- Prefer explicit naming and small focused classes.
- Keep files cohesive: one primary class per file unless tightly coupled types (e.g., enum) are very small.
- Add XML docs only for non-obvious public APIs.

## Data and security
- Never hardcode production secrets; use configuration and environment variables.
- Favor UTC (`DateTimeOffset.UtcNow`) for timestamps.
- Include indexes and constraints for uniqueness and performance when adding EF configurations.

## API conventions
- Use versioned routes once multiple public endpoints are added.
- Keep controllers thin and delegate behavior to Application layer.

## Validation and testing
- Add unit tests for domain and application behavior.
- Add integration tests for persistence and API edges as features are introduced.

## Workflow
- Update `README.md` whenever setup or architecture changes.
- Run `dotnet build` and relevant tests before finishing tasks when the SDK is available.
