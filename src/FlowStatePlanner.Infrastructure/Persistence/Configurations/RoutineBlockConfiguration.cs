using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowStatePlanner.Infrastructure.Persistence.Configurations;

public sealed class RoutineBlockConfiguration : IEntityTypeConfiguration<RoutineBlock>
{
    public void Configure(EntityTypeBuilder<RoutineBlock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => new { x.RoutineTemplateId, x.IsDeleted, x.SortOrder });
    }
}
