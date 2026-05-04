using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowStatePlanner.Infrastructure.Persistence.Configurations;

public sealed class DailyPlanConfiguration : IEntityTypeConfiguration<DailyPlan>
{
    public void Configure(EntityTypeBuilder<DailyPlan> builder)
    {
        builder.ToTable("daily_plans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlanDate).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.PlanDate }).IsUnique();
        builder.HasMany(x => x.Items).WithOne(x => x.DailyPlan).HasForeignKey(x => x.DailyPlanId).OnDelete(DeleteBehavior.Cascade);
    }
}
