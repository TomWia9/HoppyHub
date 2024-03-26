using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;
using SharedUtilities.Interfaces;

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
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes BeerDeletedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    public BeerDeletedConsumer(IApplicationDbContext context, IStorageContainerService storageContainerService)
    {
        _context = context;
        _storageContainerService = storageContainerService;
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
            var beerOpinionsImagesPath = $"Opinions/{beer.BreweryId}/{beer.Id}";

            _context.Beers.Remove(beer);
            await _storageContainerService.DeleteFromPathAsync(beerOpinionsImagesPath);

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}