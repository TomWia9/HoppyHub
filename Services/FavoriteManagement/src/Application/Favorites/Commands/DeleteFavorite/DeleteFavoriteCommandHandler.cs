using Application.Common.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.Favorites.Commands.DeleteFavorite;

/// <summary>
///     DeleteFavoriteCommand handler.
/// </summary>
public class DeleteFavoriteCommandHandler : IRequestHandler<DeleteFavoriteCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes DeleteFavoriteCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteFavoriteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _currentUserService = currentUserService;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles DeleteFavoriteCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteFavoriteCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var entity = await _context.Favorites.FirstOrDefaultAsync(x =>
                x.BeerId == request.BeerId && x.CreatedBy == currentUserId,
            cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(
                $"Beer with \"{request.BeerId}\" id has been not found in favorites of user with \"{currentUserId}\" id.");
        }

        _context.Favorites.Remove(entity);
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