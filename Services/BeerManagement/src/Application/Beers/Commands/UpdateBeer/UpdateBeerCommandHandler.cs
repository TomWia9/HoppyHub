using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.Beers.Commands.UpdateBeer;

/// <summary>
///     UpdateBeerCommand handler.
/// </summary>
public class UpdateBeerCommandHandler : IRequestHandler<UpdateBeerCommand>
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
    ///     Initializes the UpdateBeerCommandHandler.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public UpdateBeerCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles UpdateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateBeerCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Breweries.AnyAsync(x => x.Id == request.BreweryId, cancellationToken))
        {
            throw new NotFoundException(nameof(Brewery), request.BreweryId);
        }

        if (!await _context.BeerStyles.AnyAsync(x => x.Id == request.BeerStyleId, cancellationToken))
        {
            throw new NotFoundException(nameof(BeerStyle), request.BeerStyleId);
        }

        var entity = await _context.Beers.Include(x => x.Brewery)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Beer), request.Id);
        }

        entity.Name = request.Name;
        entity.BreweryId = request.BreweryId;
        entity.AlcoholByVolume = request.AlcoholByVolume;
        entity.Description = request.Description;
        entity.Composition = request.Composition;
        entity.Blg = request.Blg;
        entity.BeerStyleId = request.BeerStyleId;
        entity.Ibu = request.Ibu;
        entity.ReleaseDate = request.ReleaseDate;

        await _context.SaveChangesAsync(cancellationToken);

        var beerUpdatedEvent = new BeerUpdated
        {
            Id = entity.Id,
            Name = entity.Name,
            BreweryId = entity.BreweryId,
            BreweryName = entity.Brewery!.Name
        };

        await _publishEndpoint.Publish(beerUpdatedEvent, cancellationToken);
    }
}