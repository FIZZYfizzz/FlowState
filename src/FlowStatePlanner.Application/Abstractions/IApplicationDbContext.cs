using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<TaskItem> TaskItems { get; }
    DbSet<RoutineTemplate> RoutineTemplates { get; }
    DbSet<RoutineBlock> RoutineBlocks { get; }
    DbSet<DailyPlan> DailyPlans { get; }
    DbSet<DailyPlanItem> DailyPlanItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
