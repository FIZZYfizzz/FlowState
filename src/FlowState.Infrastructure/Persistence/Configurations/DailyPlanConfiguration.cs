using FlowState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowState.Infrastructure.Persistence.Configurations;

public class DailyPlanConfiguration : IEntityTypeConfiguration<DailyPlan>
{
    public void Configure(EntityTypeBuilder<DailyPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlanDate).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.UserId, x.PlanDate }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.DailyPlans)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
