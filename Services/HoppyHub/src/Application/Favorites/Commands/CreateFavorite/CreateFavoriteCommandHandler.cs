using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    public CreateFavoriteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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
    }
}