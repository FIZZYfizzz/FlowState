using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowStatePlanner.Infrastructure.Persistence.Configurations;

public sealed class DailyPlanItemConfiguration : IEntityTypeConfiguration<DailyPlanItem>
{
    public void Configure(EntityTypeBuilder<DailyPlanItem> builder)
    {
        builder.ToTable("daily_plan_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.HasOne(x => x.TaskItem).WithMany().HasForeignKey(x => x.TaskItemId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.RoutineBlock).WithMany().HasForeignKey(x => x.RoutineBlockId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => new { x.DailyPlanId, x.TaskItemId, x.SourceType });
    }
}
