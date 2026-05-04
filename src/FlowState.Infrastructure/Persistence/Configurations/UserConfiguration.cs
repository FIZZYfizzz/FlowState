using FlowState.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowState.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(320);
        builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(120);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.UpdatedAtUtc).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
