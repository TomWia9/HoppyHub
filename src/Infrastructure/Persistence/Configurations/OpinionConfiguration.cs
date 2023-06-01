using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The opinion entity configuration.
/// </summary>
public class OpinionConfiguration : BaseConfiguration<Opinion>
{
    /// <summary>
    ///     Configures the opinion entity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public override void Configure(EntityTypeBuilder<Opinion> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Rating).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.Created).IsRequired();
        builder.Property(x => x.LastModifiedBy).IsRequired();
        builder.Property(x => x.LastModified).IsRequired();
    }
}