using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Beers.EventConsumers;

/// <summary>
///     BeerUpdated consumer.
/// </summary>
public class BeerUpdatedConsumer : IConsumer<BeerUpdated>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeerUpdatedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeerUpdatedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BeerUpdated event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerUpdated> context)
    {
        var message = context.Message;

        var beer = await _context.Beers.FindAsync(message.Id);

        if (beer is not null)
        {
            beer.Name = message.Name;
            beer.BreweryId = message.BreweryId;

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}