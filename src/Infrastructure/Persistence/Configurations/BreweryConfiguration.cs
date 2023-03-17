using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The brewery entity configuration.
/// </summary>
public class BreweryConfiguration : BaseConfiguration<Brewery>
{
    /// <summary>
    ///     Configures the brewery entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<Brewery> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(5000);
        builder.Property(x => x.FoundationYear).IsRequired();
        builder.Property(x => x.WebsiteUrl).HasMaxLength(200);

        builder.HasOne(b => b.Address)
            .WithOne(a => a.Brewery)
            .HasForeignKey<Address>(a => a.BreweryId);
    }
}