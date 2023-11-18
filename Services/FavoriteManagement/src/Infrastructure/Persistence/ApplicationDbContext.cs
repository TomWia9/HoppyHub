using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
///     ApplicationDbContext class.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    ///     The auditable entity save changes interceptor.
    /// </summary>
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    /// <summary>
    ///     Initializes ApplicationDbContext.
    /// </summary>
    /// <param name="options">The DbContextOptions></param>
    /// <param name="auditableEntitySaveChangesInterceptor">The AuditableEntitySaveChangesInterceptor</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    /// <summary>
    ///     The beers.
    /// </summary>
    public DbSet<Beer> Beers => Set<Beer>();

    /// <summary>
    ///     Saves changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    ///     OnModelCreating override.
    /// </summary>
    /// <param name="builder">The ModelBuilder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    /// <summary>
    ///     OnConfiguring override.
    /// </summary>
    /// <param name="optionsBuilder">The DbContextOptionsBuilder</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }
}