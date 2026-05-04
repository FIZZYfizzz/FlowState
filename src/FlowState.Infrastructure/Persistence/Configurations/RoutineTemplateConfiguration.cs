using FlowState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowState.Infrastructure.Persistence.Configurations;

public class RoutineTemplateConfiguration : IEntityTypeConfiguration<RoutineTemplate>
{
    public void Configure(EntityTypeBuilder<RoutineTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.RoutineTemplates)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
