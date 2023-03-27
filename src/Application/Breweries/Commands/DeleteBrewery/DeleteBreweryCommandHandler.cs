using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Breweries.Commands.DeleteBrewery;

/// <summary>
///     DeleteBreweryCommand handler.
/// </summary>
public class DeleteBreweryCommandHandler : IRequestHandler<DeleteBreweryCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes DeleteBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    public DeleteBreweryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles DeleteBreweryCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBreweryCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Breweries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Brewery), request.Id);
        }

        _context.Breweries.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}