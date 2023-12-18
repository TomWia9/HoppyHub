using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Common.Interfaces;

/// <summary>
///     ApplicationDbContext interface.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    ///     The beers.
    /// </summary>
    DbSet<Beer> Beers { get; }

    /// <summary>
    ///     The favorites.
    /// </summary>
    DbSet<Favorite> Favorites { get; }

    /// <summary>
    ///     The users.
    /// </summary>
    DbSet<User> Users { get; }

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