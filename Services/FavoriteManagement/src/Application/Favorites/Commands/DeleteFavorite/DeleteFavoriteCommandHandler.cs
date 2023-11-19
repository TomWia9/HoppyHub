﻿using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    ///     Initializes DeleteFavoriteCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    public DeleteFavoriteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
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
    }
}