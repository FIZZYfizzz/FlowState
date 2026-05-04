using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowStatePlanner.Infrastructure.Persistence.Configurations;

public sealed class RoutineTemplateConfiguration : IEntityTypeConfiguration<RoutineTemplate>
{
    public void Configure(EntityTypeBuilder<RoutineTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.AppliesToDays)
            .HasConversion(
                days => string.Join(',', days.Select(d => (int)d)),
                serialized => string.IsNullOrWhiteSpace(serialized)
                    ? new List<DayOfWeek>()
                    : serialized.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => (DayOfWeek)int.Parse(v)).ToList());

        builder.HasMany(x => x.Blocks)
            .WithOne(x => x.RoutineTemplate)
            .HasForeignKey(x => x.RoutineTemplateId);

        builder.HasIndex(x => new { x.UserId, x.IsDeleted });
    }
}
