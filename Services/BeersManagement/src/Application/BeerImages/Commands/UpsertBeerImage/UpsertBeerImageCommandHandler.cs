using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     UpsertBeerImageCommand handler.
/// </summary>
public class UpsertBeerImageCommandHandler : IRequestHandler<UpsertBeerImageCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes UpsertBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public UpsertBeerImageCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles UpsertBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpsertBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync(new object?[] { request.BeerId },
            cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }
        
        var imageCreatedEvent = new ImageCreated
        {
            Path = $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}",
            Image = request.Image
        };

        await _publishEndpoint.Publish(imageCreatedEvent, cancellationToken);
        
        //TODO: Ensure that image uploaded and add/update entity

        // var entity = await _context.BeerImages.FirstOrDefaultAsync(x => x.BeerId == request.BeerId,
        //     cancellationToken: cancellationToken);
        //
        // if (entity is null)
        // {
        //     entity = new BeerImage
        //     {
        //         BeerId = message.BeerId,
        //         ImageUri = message.ImageUri,
        //         TempImage = false
        //     };
        //
        //     await _context.BeerImages.AddAsync(entity);
        // }
        // else
        // {
        //     entity.ImageUri = message.ImageUri;
        //     entity.TempImage = false;
        // }
        //
        // await _context.SaveChangesAsync(CancellationToken.None);
    }
}