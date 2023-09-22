using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;

/// <summary>
///     ApplicationDbContext interface.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    ///     Provides access to database related information and operations for this context.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    ///     Saves changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}