using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.Favorites.Commands.CreateFavorite;

/// <summary>
///     CreateFavoriteCommand handler.
/// </summary>
public class CreateFavoriteCommandHandler : IRequestHandler<CreateFavoriteCommand>
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
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public CreateFavoriteCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles CreateFavoriteCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(CreateFavoriteCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Beers.AnyAsync(x => x.Id == request.BeerId, cancellationToken))
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var entity = new Favorite
        {
            BeerId = request.BeerId
        };

        await _context.Favorites.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var newBeerFavoritesCount =
            await _context.Favorites.CountAsync(x => x.BeerId == request.BeerId, cancellationToken);

        var favoritesCountChangedEvent = new BeerFavoritesCountChanged
        {
            BeerId = request.BeerId,
            FavoritesCount = newBeerFavoritesCount
        };

        await _publishEndpoint.Publish(favoritesCountChangedEvent, cancellationToken);
    }
}