using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace Application.Beers.EventConsumers;

/// <summary>
///     BeerDeleted consumer.
/// </summary>
public class BeerDeletedConsumer : IConsumer<BeerDeleted>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeerDeletedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeerDeletedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BeerDeleted event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerDeleted> context)
    {
        var message = context.Message;

        var beer = await _context.Beers.FindAsync(message.Id);

        if (beer is not null)
        {
            _context.Beers.Remove(beer);

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}