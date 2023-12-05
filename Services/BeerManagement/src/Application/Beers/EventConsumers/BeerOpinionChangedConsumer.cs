using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Beers.EventConsumers;

/// <summary>
///     BeerOpinionChanged consumer.
/// </summary>
public class BeerOpinionChangedConsumer : IConsumer<BeerOpinionChanged>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeerOpinionChangedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeerOpinionChangedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BeerOpinionChanged event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerOpinionChanged> context)
    {
        var message = context.Message;
        var beer = await _context.Beers.FindAsync(message.BeerId);

        if (beer is not null)
        {
            beer.OpinionsCount = message.OpinionsCount;
            beer.Rating = message.NewBeerRating;

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}