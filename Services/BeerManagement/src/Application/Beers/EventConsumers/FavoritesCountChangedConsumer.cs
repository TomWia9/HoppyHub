using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Beers.EventConsumers;

/// <summary>
///     FavoritesCountChanged consumer.
/// </summary>
public class FavoritesCountChangedConsumer : IConsumer<FavoritesCountChanged>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes FavoritesCountChangedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public FavoritesCountChangedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes FavoritesCountChanged event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<FavoritesCountChanged> context)
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