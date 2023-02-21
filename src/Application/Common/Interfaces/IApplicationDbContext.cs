using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Beer> Beers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}