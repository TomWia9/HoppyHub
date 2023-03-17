using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The AddressConfiguration class.
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    /// <summary>
    ///     Configures the address entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(x => x.Street).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Number).IsRequired().HasMaxLength(10);
        builder.Property(x => x.PostCode).IsRequired().HasMaxLength(8);
        builder.Property(x => x.City).IsRequired().HasMaxLength(50);
        builder.Property(x => x.State).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(50);
        builder.Property(x => x.BreweryId).IsRequired();
    }
}