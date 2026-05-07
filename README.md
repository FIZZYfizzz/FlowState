# FlowState Planner Backend Foundation

Initial backend foundation for FlowState Planner built as a modular monolith using ASP.NET Core Web API (.NET 8), EF Core, and PostgreSQL.

## Solution structure

- `src/FlowStatePlanner.Api` - API bootstrap, middleware, Swagger, and JWT scaffolding.
- `src/FlowStatePlanner.Application` - Application use-cases, DTOs, interfaces, validation, and orchestration logic.
- `src/FlowStatePlanner.Domain` - Core domain entities and domain rules.
- `src/FlowStatePlanner.Infrastructure` - EF Core DbContext, PostgreSQL persistence, migrations, and infrastructure wiring.
- `tests/FlowStatePlanner.Tests` - Unit and smoke tests for backend behavior.

## Implemented in this foundation

- Domain entities:
  - `User`
  - `TaskItem`
  - `RoutineTemplate`
  - `RoutineBlock`
  - `DailyPlan`
  - `DailyPlanItem`
- `FlowStatePlannerDbContext` with all required `DbSet<>` definitions.
- PostgreSQL configuration via the `AddInfrastructure` extension.
- EF Core design-time DbContext factory for CLI migrations.
- Initial EF Core migration in the Infrastructure project.
- Development Swagger/OpenAPI setup.
- JWT authentication scaffolding (issuer/audience/signing key setup only).
- `GET /health` minimal endpoint.

## Prerequisites

- .NET SDK 8.x
- Docker Desktop or Docker Engine with Docker Compose for the local PostgreSQL container
- EF Core CLI tools (`dotnet ef`)

If `dotnet ef` is not installed, install it with:

```bash
dotnet tool install --global dotnet-ef --version 8.*
```

## Local PostgreSQL setup

The repository includes `docker-compose.yml` for local development. It starts PostgreSQL 16 with:

- Container/service name: `flowstate-postgres`
- Database: `flowstate_planner`
- User: `flowstate`
- Password: `flowstate_dev_password`
- Host port: `5432`
- Named volume: `flowstate-postgres-data`

The API development connection string is configured in `src/FlowStatePlanner.Api/appsettings.Development.json`:

```text
Host=localhost;Port=5432;Database=flowstate_planner;Username=flowstate;Password=flowstate_dev_password
```

You can override the connection string with an environment variable when needed:

```bash
export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=flowstate_planner;Username=flowstate;Password=flowstate_dev_password"
```

A `.env.example` file documents the local development database variables. Do not commit real production secrets.

## Setup and run locally

1. Restore dependencies:

   ```bash
   dotnet restore FlowStatePlanner.sln
   ```

2. Start PostgreSQL:

   ```bash
   docker compose up -d
   ```

3. Apply EF Core migrations:

   ```bash
   dotnet ef database update --project src/FlowStatePlanner.Infrastructure --startup-project src/FlowStatePlanner.Api
   ```

4. Run the API:

   ```bash
   dotnet run --project src/FlowStatePlanner.Api
   ```

5. Verify health:

   ```bash
   curl http://localhost:5000/health
   ```

   If the API chooses a different URL, use the URL printed by `dotnet run` and append `/health`.

6. Open Swagger in development:

   ```text
   http://localhost:5000/swagger
   ```

   If the API chooses a different URL, use the URL printed by `dotnet run` and append `/swagger`.

## Reset the local database

To stop PostgreSQL while keeping the local data volume:

```bash
docker compose down
```

To fully reset local database data, remove the named volume and then start PostgreSQL again:

```bash
docker compose down -v
docker compose up -d
dotnet ef database update --project src/FlowStatePlanner.Infrastructure --startup-project src/FlowStatePlanner.Api
```

## EF Core migrations

Migrations live in `src/FlowStatePlanner.Infrastructure/Persistence/Migrations` and use the `planner` schema. To add a new migration after changing EF Core mappings or domain persistence shape:

```bash
dotnet ef migrations add <MigrationName> --project src/FlowStatePlanner.Infrastructure --startup-project src/FlowStatePlanner.Api --output-dir Persistence/Migrations
```

Apply migrations with:

```bash
dotnet ef database update --project src/FlowStatePlanner.Infrastructure --startup-project src/FlowStatePlanner.Api
```

## Continuous integration

GitHub Actions verifies the backend with the `.github/workflows/backend-ci.yml` workflow on pull requests targeting `main` and pushes to `main`. The workflow runs on `ubuntu-latest`, installs .NET 8, restores `FlowStatePlanner.sln`, builds it in Release mode, and runs the solution tests. NuGet package caching is enabled with `actions/cache` using the solution and project files as the cache key inputs. CI does not require a live PostgreSQL database yet.

## Notes

- Authentication is intentionally scaffolded, not fully implemented.
- This baseline is prepared for incremental addition of application use-cases, validation, and migrations.
