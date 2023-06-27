using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     DeleteBeerImageCommand handler.
/// </summary>
public class DeleteBeerImageCommandHandler : IRequestHandler<DeleteBeerImageCommand>
{
    /// <summary>
    ///     The beer images service.
    /// </summary>
    private readonly IBeersImagesService _beerImagesService;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes DeleteBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="beerImagesService">The beer images service</param>
    public DeleteBeerImageCommandHandler(IApplicationDbContext context, IBeersImagesService beerImagesService)
    {
        _context = context;
        _beerImagesService = beerImagesService;
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
            var beerImageUri = beer.BeerImage.ImageUri;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                beer.BeerImage.ImageUri = _beerImagesService.GetTempBeerImageUri();
                beer.BeerImage.TempImage = true;
                await _context.SaveChangesAsync(cancellationToken);

                await _beerImagesService.DeleteImageAsync(beerImageUri);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}