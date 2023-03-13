using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The beer entity configuration.
/// </summary>
public class BeerConfiguration : BaseConfiguration<Beer>
{
    /// <summary>
    ///     Configures the beer entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<Beer> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Brewery).IsRequired().HasMaxLength(200);
        builder.Property(x => x.AlcoholByVolume).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(3000);
        builder.Property(x => x.Style).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(50);
    }
}