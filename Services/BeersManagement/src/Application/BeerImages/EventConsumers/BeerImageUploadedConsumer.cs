using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.EventConsumers;

/// <summary>
///     BeerImageUploaded consumer.
/// </summary>
public class BeerImageUploadedConsumer : IConsumer<BeerImageUploaded>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes BeerImageUploadedConsumer.
    /// </summary>
    /// <param name="context">The database context</param>
    public BeerImageUploadedConsumer(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Consumes BeerImageUploaded event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<BeerImageUploaded> context)
    {
        var message = context.Message;

        var beer = await _context.Beers.FindAsync(message.BeerId);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), message.BeerId);
        }

        var entity = await _context.BeerImages.FirstOrDefaultAsync(x => x.BeerId == message.BeerId);

        // Create new BeerImage for requested beer if it was not created for some reason.
        if (entity is null)
        {
            entity = new BeerImage
            {
                BeerId = message.BeerId,
                ImageUri = message.ImageUri,
                TempImage = false
            };

            await _context.BeerImages.AddAsync(entity);
        }
        else
        {
            entity.ImageUri = message.ImageUri;
            entity.TempImage = false;
        }

        await _context.SaveChangesAsync(CancellationToken.None);
    }
}