using Domain.Common;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedUtilities.Interfaces;

namespace Infrastructure.Persistence.Interceptors;

/// <summary>
///     The AuditableEntitySaveChangesInterceptor class.
/// </summary>
public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    /// <summary>
    ///     Current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The time provider.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    ///     Initializes AuditableEntitySaveChangesInterceptor.
    /// </summary>
    /// <param name="currentUserService">Current user service</param>
    /// <param name="timeProvider">The time provider</param>
    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService, TimeProvider timeProvider)
    {
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    /// <summary>
    ///     SavingChanges override
    /// </summary>
    /// <param name="eventData">The DbContextEventData</param>
    /// <param name="result">The InterceptionResult</param>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    ///     SavingChangesAsync override
    /// </summary>
    /// <param name="eventData">The DbContextEventData</param>
    /// <param name="result">The InterceptionResult</param>
    /// <param name="cancellationToken">The CancellationToken</param>
    /// <returns></returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    ///     Updates entities with audit columns
    /// </summary>
    /// <param name="context">The database context</param>
    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = _currentUserService.UserId;
                entry.Entity.Created = _timeProvider.GetUtcNow();
            }

            if (entry.State is EntityState.Added or EntityState.Modified ||
                entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy = _currentUserService.UserId;
                entry.Entity.LastModified = _timeProvider.GetUtcNow();
            }
        }
    }
}