using FlowState.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Infrastructure.Persistence;

public class FlowStateDbContext(DbContextOptions<FlowStateDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<RoutineTemplate> RoutineTemplates => Set<RoutineTemplate>();
    public DbSet<RoutineBlock> RoutineBlocks => Set<RoutineBlock>();
    public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();
    public DbSet<DailyPlanItem> DailyPlanItems => Set<DailyPlanItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowStateDbContext).Assembly);
    }
}
