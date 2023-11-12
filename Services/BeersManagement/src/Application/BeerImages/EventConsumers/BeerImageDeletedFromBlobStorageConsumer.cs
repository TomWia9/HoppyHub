using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.EventConsumers;

/// <summary>
///     BeerImageDeletedFromBlobStorage consumer.
/// </summary>
public class BeerImageDeletedFromBlobStorageConsumer : IConsumer<BeerImageDeletedFromBlobStorage>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The app configuration.
    /// </summary>
    private readonly IAppConfiguration _appConfiguration;

    /// <summary>
    ///     Initializes BeerImageDeletedFromBlobStorageConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="appConfiguration">The app configuration</param>
    public BeerImageDeletedFromBlobStorageConsumer(IApplicationDbContext context, IAppConfiguration appConfiguration)
    {
        _context = context;
        _appConfiguration = appConfiguration;
    }

    /// <summary>
    ///     Consumes BeerImageDeletedFromBlobStorage event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerImageDeletedFromBlobStorage> context)
    {
        var message = context.Message;

        var beer = await _context.Beers.Include(x => x.BeerImage)
            .FirstOrDefaultAsync(x => x.Id == message.BeerId);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), message.BeerId);
        }

        if (beer.BeerImage is { TempImage: false, ImageUri: not null })
        {
            beer.BeerImage.ImageUri = _appConfiguration.TempBeerImageUri;
            beer.BeerImage.TempImage = true;

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}