using Application.Common.Interfaces;
using MassTransit;
using SharedEvents.Events;
using SharedUtilities.Interfaces;

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
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes BreweryDeletedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    public BreweryDeletedConsumer(IApplicationDbContext context, IStorageContainerService storageContainerService)
    {
        _context = context;
        _storageContainerService = storageContainerService;
    }

    /// <summary>
    ///     Consumes BreweryDeleted event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BreweryDeleted> context)
    {
        var message = context.Message;
        var breweryBeersOpinionsImagesPath = $"Opinions/{message.Id}";
        var breweryBeers = _context.Beers.Where(x => x.BreweryId == message.Id);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Beers.RemoveRange(breweryBeers);
            await _context.SaveChangesAsync(CancellationToken.None);

            await _storageContainerService.DeleteFromPathAsync(breweryBeersOpinionsImagesPath);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}