using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Beers.Commands.DeleteBeer;

/// <summary>
///     DeleteBeerCommand handler
/// </summary>
public class DeleteBeerCommandHandler : IRequestHandler<DeleteBeerCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes DeleteBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    public DeleteBeerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles DeleteBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Beers.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Beer), request.Id);
        }

        _context.Beers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}