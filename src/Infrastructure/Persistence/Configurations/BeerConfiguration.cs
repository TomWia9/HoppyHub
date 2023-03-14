using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200).HasColumnOrder(1);
        builder.Property(x => x.Brewery).IsRequired().HasMaxLength(200).HasColumnOrder(2);
        builder.Property(x => x.Style).IsRequired().HasMaxLength(50).HasColumnOrder(3);
        builder.Property(x => x.AlcoholByVolume).IsRequired().HasColumnOrder(4);
        builder.Property(x => x.SpecificGravity).HasColumnOrder(5);
        builder.Property(x => x.Blg).HasColumnOrder(6);
        builder.Property(x => x.Plato).HasColumnOrder(7);
        builder.Property(x => x.Ibu).HasColumnOrder(8);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(50).HasColumnOrder(9);
        builder.Property(x => x.Description).HasMaxLength(3000).HasColumnOrder(10);
    }
}