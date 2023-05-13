﻿using Domain.Entities;
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
        builder.Property(x => x.BreweryId).IsRequired();
        
        builder.HasMany(x => x.Opinions)
            .WithOne(x => x.Beer)
            .IsRequired();
        
        builder.HasMany(x => x.Favorites)
            .WithOne(x => x.Beer)
            .IsRequired();
    }
}