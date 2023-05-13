using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The favorite entity configuration.
/// </summary>
public class FavoriteConfiguration : BaseConfiguration<Favorite>
{
    /// <summary>
    ///     Configures the favorite entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<Favorite> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.BeerId).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.Created).IsRequired();
        builder.Property(x => x.LastModifiedBy).IsRequired();
        builder.Property(x => x.LastModified).IsRequired();
        //TODO verify that hasMaxLength stay as it is in BaseConfiguration
    }
}