using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The user entity configuration.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    ///     Configures the user entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Username).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(15);

        builder.HasMany(x => x.Favorites)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy);
    }
}