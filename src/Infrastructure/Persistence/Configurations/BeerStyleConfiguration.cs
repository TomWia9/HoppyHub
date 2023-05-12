using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The beer style entity configuration.
/// </summary>
public class BeerStyleConfiguration : BaseConfiguration<BeerStyle>
{
    /// <summary>
    ///     Configures the beer style entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<BeerStyle> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.CountryOfOrigin).IsRequired().HasMaxLength(50);
        
        builder.HasMany(x => x.Beers)
            .WithOne(x => x.BeerStyle)
            .IsRequired();
    }
}