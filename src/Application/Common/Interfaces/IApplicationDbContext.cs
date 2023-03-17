using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
    ///     The breweries.
    /// </summary>
    DbSet<Brewery> Breweries { get; }
    
    /// <summary>
    ///     The addresses.
    /// </summary>
    DbSet<Address> Addresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}