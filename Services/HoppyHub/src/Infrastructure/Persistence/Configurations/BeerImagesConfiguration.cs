using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The BeerImagesConfiguration class.
/// </summary>
public class BeerImagesConfiguration : IEntityTypeConfiguration<BeerImage>
{
    /// <summary>
    ///     Configures the beer image entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public void Configure(EntityTypeBuilder<BeerImage> builder)
    {
        builder.Property(x => x.ImageUri).IsRequired();
        builder.Property(x => x.TempImage).IsRequired();
        builder.Property(x => x.BeerId).IsRequired();
    }
}