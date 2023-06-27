using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     UpdateBeerStyleCommand handler.
/// </summary>
public class UpdateBeerStyleCommandHandler : IRequestHandler<UpdateBeerStyleCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UpdateBeerStyleCommandHandler.
    /// </summary>
    public UpdateBeerStyleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles UpdateBeerStyleCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateBeerStyleCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.BeerStyles.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(BeerStyle), request.Id);
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.CountryOfOrigin = request.CountryOfOrigin;

        await _context.SaveChangesAsync(cancellationToken);
    }
}