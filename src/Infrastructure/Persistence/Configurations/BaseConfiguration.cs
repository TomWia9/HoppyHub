using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
///     The BaseConfiguration class.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseAuditableEntity
{
    /// <summary>
    ///     Configures BaseAuditableEntity.
    /// </summary>
    /// <param name="builder">The builder</param>
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnOrder(0);
        builder.Property(x => x.Created).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(40);
        builder.Property(x => x.LastModified).HasMaxLength(50);
        builder.Property(x => x.LastModifiedBy).HasMaxLength(40);
    }
}