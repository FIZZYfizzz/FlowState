# FlowState Planner Backend Foundation

Initial backend foundation for FlowState Planner built as a modular monolith using ASP.NET Core Web API (.NET 8), EF Core, and PostgreSQL.

## Solution structure

- `src/FlowStatePlanner.Api` - API bootstrap, middleware, Swagger, and JWT scaffolding.
- `src/FlowStatePlanner.Application` - Application layer placeholder.
- `src/FlowStatePlanner.Domain` - Core domain entities and enums.
- `src/FlowStatePlanner.Infrastructure` - EF Core DbContext and persistence wiring.
- `tests/FlowStatePlanner.Tests` - Basic test project scaffold.

## Implemented in this foundation

- Domain entities:
  - `User`
  - `TaskItem`
  - `RoutineTemplate`
  - `RoutineBlock`
  - `DailyPlan`
  - `DailyPlanItem`
- `FlowStatePlannerDbContext` with all required `DbSet<>` definitions.
- PostgreSQL configuration via `AddInfrastructure` extension.
- Development Swagger/OpenAPI setup.
- JWT authentication scaffolding (issuer/audience/signing key setup only).
- `GET /health` minimal endpoint.

## Prerequisites

- .NET SDK 8.x
- PostgreSQL 14+

## Setup

1. Configure `src/FlowStatePlanner.Api/appsettings.json` for local credentials or override with environment variables.
2. Restore and build:
   - `dotnet restore`
   - `dotnet build FlowStatePlanner.sln`
3. Add first migration and apply database:
   - `dotnet ef migrations add InitialCreate --project src/FlowStatePlanner.Infrastructure --startup-project src/FlowStatePlanner.Api`
   - `dotnet ef database update --project src/FlowStatePlanner.Infrastructure --startup-project src/FlowStatePlanner.Api`
4. Run API:
   - `dotnet run --project src/FlowStatePlanner.Api`
5. Open Swagger in development:
   - `https://localhost:xxxx/swagger`

## Notes

- Authentication is intentionally scaffolded, not fully implemented.
- This baseline is prepared for incremental addition of application use-cases, validation, and migrations.
