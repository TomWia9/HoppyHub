using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The application user entity configuration.
/// </summary>
public class OpinionConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <summary>
    ///     Configures the application user entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.Created).IsRequired().HasMaxLength(50);
    }
}