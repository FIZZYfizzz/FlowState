using FlowState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowState.Infrastructure.Persistence.Configurations;

public class DailyPlanItemConfiguration : IEntityTypeConfiguration<DailyPlanItem>
{
    public void Configure(EntityTypeBuilder<DailyPlanItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();

        builder.HasOne(x => x.DailyPlan)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.DailyPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.TaskItem)
            .WithMany()
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
