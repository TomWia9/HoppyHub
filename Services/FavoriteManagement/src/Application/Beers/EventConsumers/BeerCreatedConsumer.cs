using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;

namespace Application.Beers.EventConsumers;

/// <summary>
///     BeerCreated consumer.
/// </summary>
public class BeerCreatedConsumer : IConsumer<BeerCreated>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeerCreatedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeerCreatedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BeerCreated event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerCreated> context)
    {
        var message = context.Message;

        var beer = new Beer
        {
            Id = message.Id,
            Name = message.Name,
            BreweryName = message.BreweryName,
            BreweryId = message.BreweryId
        };

        if (!await _context.Beers.AnyAsync(x => x.Id == beer.Id))
        {
            await _context.Beers.AddAsync(beer);
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}