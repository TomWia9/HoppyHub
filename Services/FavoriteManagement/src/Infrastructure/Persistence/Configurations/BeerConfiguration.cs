using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The beer entity configuration.
/// </summary>
public class BeerConfiguration : IEntityTypeConfiguration<Beer>
{
    /// <summary>
    ///     Configures the beer entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public void Configure(EntityTypeBuilder<Beer> builder)
    {
        builder.HasMany(x => x.Favorites)
            .WithOne(x => x.Beer)
            .IsRequired();
    }
}