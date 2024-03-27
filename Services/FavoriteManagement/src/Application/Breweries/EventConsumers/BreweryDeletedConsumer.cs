using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Breweries.EventConsumers;

/// <summary>
///     BreweryDeleted consumer.
/// </summary>
public class BreweryDeletedConsumer : IConsumer<BreweryDeleted>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BreweryDeletedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BreweryDeletedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BreweryDeleted event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BreweryDeleted> context)
    {
        var message = context.Message;
        var breweryBeers = _context.Beers.Where(x => x.BreweryId == message.Id);

        _context.Beers.RemoveRange(breweryBeers);
        await _context.SaveChangesAsync(CancellationToken.None);
    }
}