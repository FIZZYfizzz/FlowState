using FlowState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowState.Infrastructure.Persistence.Configurations;

public class RoutineBlockConfiguration : IEntityTypeConfiguration<RoutineBlock>
{
    public void Configure(EntityTypeBuilder<RoutineBlock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.DurationMinutes).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => new { x.RoutineTemplateId, x.SortOrder });

        builder.HasOne(x => x.RoutineTemplate)
            .WithMany(x => x.Blocks)
            .HasForeignKey(x => x.RoutineTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
