using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     UpsertBeerImageCommand handler.
/// </summary>
public class UpsertBeerImageCommandHandler : IRequestHandler<UpsertBeerImageCommand, string>
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
    ///     Initializes UpsertBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="beerImagesService">The beer images service</param>
    public UpsertBeerImageCommandHandler(IApplicationDbContext context, IBeersImagesService beerImagesService)
    {
        _context = context;
        _beerImagesService = beerImagesService;
    }

    /// <summary>
    ///     Handles UpsertBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<string> Handle(UpsertBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync(new object?[] { request.BeerId },
            cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var entity = await _context.BeerImages.FirstOrDefaultAsync(x => x.BeerId == request.BeerId,
            cancellationToken);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var imagePath = _beerImagesService.CreateImagePath(request.Image!, beer.BreweryId, beer.Id);
            var imageUri = await _beerImagesService.UploadImageAsync(imagePath, request.Image!);

            // Create new BeerImage for requested beer if it was not created for some reason.
            if (entity is null)
            {
                entity = new BeerImage
                {
                    BeerId = request.BeerId,
                    ImageUri = imageUri,
                    TempImage = false
                };

                await _context.BeerImages.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.ImageUri = imageUri;
                entity.TempImage = false;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return imageUri;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}