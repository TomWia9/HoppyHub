using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The primary beer style entity configuration.
/// </summary>
public class PrimaryBeerStyleConfiguration : BaseConfiguration<PrimaryBeerStyle>
{
    /// <summary>
    ///     Configures the primary beer style entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<PrimaryBeerStyle> builder)
    {
        base.Configure(builder);
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        
        builder.HasMany(x => x.BeerStyles)
            .WithOne(x => x.PrimaryBeerStyle)
            .IsRequired();
    }
}