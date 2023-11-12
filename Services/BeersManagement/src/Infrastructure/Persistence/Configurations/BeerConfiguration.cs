using Domain.Entities;
using Infrastructure.Converters;
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

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.BeerStyleId).IsRequired();
        builder.Property(x => x.AlcoholByVolume).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(3000);
        builder.Property(x => x.Composition).HasMaxLength(300);
        builder.Property(x => x.ReleaseDate).HasConversion<DateOnlyConverter>()
            .HasColumnType("date");
        builder.Property(x => x.BreweryId).IsRequired();
        builder.Property(x => x.Rating).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.OpinionsCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.FavoritesCount).IsRequired().HasDefaultValue(0);

        builder.HasOne(x => x.BeerImage)
            .WithOne(y => y.Beer)
            .HasForeignKey<BeerImage>(y => y.BeerId);
    }
}