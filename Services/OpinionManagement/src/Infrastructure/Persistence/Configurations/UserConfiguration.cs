using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The user entity configuration.
/// </summary>
public class UserConfiguration : BaseConfiguration<User>
{
    /// <summary>
    ///     Configures the user entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Username).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Role).IsRequired().HasMaxLength(15);
        builder.Property(x => x.Created).IsRequired();
        builder.Property(x => x.LastModified).IsRequired();

        builder.HasMany(x => x.Opinions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.CreatedBy);
    }
}