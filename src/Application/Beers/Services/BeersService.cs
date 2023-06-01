using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Beers.Services;

/// <summary>
///     Beers service.
/// </summary>
public class BeersService : IBeersService
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeersService.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeersService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Calculates beer average rating.
    /// </summary>
    public async Task CalculateBeerRatingAsync(Guid beerId)
    {
        var beerRating = await _context.Opinions.Where(x => x.BeerId == beerId)
            .AverageAsync(x => x.Rating);

        var beer = await _context.Beers.FindAsync(beerId);

        if (beer == null)
        {
            throw new NotFoundException(nameof(Beer), beerId);
        }

        beer.Rating = Math.Round(beerRating, 2);
    }
}