using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.BeerStyles.Commands.DeleteBeerStyle;

/// <summary>
///     DeleteBeerStyleCommand handler.
/// </summary>
public class DeleteBeerStyleCommandHandler : IRequestHandler<DeleteBeerStyleCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes DeleteBeerStyleCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    public DeleteBeerStyleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles DeleteBeerStyleCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerStyleCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.BeerStyles.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(BeerStyle), request.Id);
        }

        _context.BeerStyles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}