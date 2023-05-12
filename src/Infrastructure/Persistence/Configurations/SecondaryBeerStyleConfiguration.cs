using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The secondary beer style entity configuration.
/// </summary>
public class SecondaryBeerStyleConfiguration : BaseConfiguration<SecondaryBeerStyle>
{
    /// <summary>
    ///     Configures the secondary beer style entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<SecondaryBeerStyle> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.CountryOfOrigin).IsRequired().HasMaxLength(50);
    }
}