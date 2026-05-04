# FlowState

Backend foundation for FlowState scheduling.

## Current scope

- Domain entities for users, tasks, routines, and daily plans.
- EF Core fluent configurations for constraints, indexes, and relationships.
- No controllers, authentication, or plan generation logic yet.

## Model hardening highlights

- Unique user email constraint.
- TaskItem index on `(UserId, DueDate)`.
- DailyPlan unique index on `(UserId, PlanDate)`.
- RoutineTemplate index on `UserId`.
- RoutineBlock index on `(RoutineTemplateId, SortOrder)`.
- Audit fields (`CreatedAtUtc`, `UpdatedAtUtc`) and soft-delete (`IsDeleted`) on root planning entities.
- Scheduling fields added for future planning (`StartTime`, `DurationMinutes`, `TaskType`, `FlexibilityType`).
