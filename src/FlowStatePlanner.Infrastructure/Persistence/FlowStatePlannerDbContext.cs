using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Infrastructure.Persistence;

public class FlowStatePlannerDbContext(DbContextOptions<FlowStatePlannerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RoutineTemplate> RoutineTemplates => Set<RoutineTemplate>();
    public DbSet<RoutineBlock> RoutineBlocks => Set<RoutineBlock>();
    public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();
    public DbSet<DailyPlanItem> DailyPlanItems => Set<DailyPlanItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("planner");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowStatePlannerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
