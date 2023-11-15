using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     DeleteBeerImageCommand handler.
/// </summary>
public class DeleteBeerImageCommandHandler : IRequestHandler<DeleteBeerImageCommand>
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
    ///     The app configuration.
    /// </summary>
    private readonly IAppConfiguration _appConfiguration;

    /// <summary>
    ///     Initializes DeleteBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    /// <param name="appConfiguration">The app configuration</param>
    public DeleteBeerImageCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint,
        IAppConfiguration appConfiguration)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _appConfiguration = appConfiguration;
    }

    /// <summary>
    ///     Handles DeleteBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.Include(x => x.BeerImage)
            .FirstOrDefaultAsync(x => x.Id == request.BeerId, cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        if (beer.BeerImage is { TempImage: false, ImageUri: not null })
        {
            var beerImageDeleted = new ImagesDeleted
            {
                Paths = new List<string>
                {
                    $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}"
                }
            };

            await _publishEndpoint.Publish(beerImageDeleted, cancellationToken);

            //TODO: Ensure that image deleted and update entity.

            beer.BeerImage.ImageUri = _appConfiguration.TempBeerImageUri;
            beer.BeerImage.TempImage = true;

            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}