using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Beers.EventConsumers;

/// <summary>
///     BeerFavoritesCountChanged consumer.
/// </summary>
public class BeerFavoritesCountChangedConsumer : IConsumer<BeerFavoritesCountChanged>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeerFavoritesCountChangedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeerFavoritesCountChangedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BeerFavoritesCountChanged event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerFavoritesCountChanged> context)
    {
        var message = context.Message;
        var beer = await _context.Beers.FindAsync(message.BeerId);

        if (beer is not null)
        {
            beer.FavoritesCount = message.FavoritesCount;
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}