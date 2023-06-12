using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     UpsertBeerImageCommand handler.
/// </summary>
public class UpsertBeerImageCommandHandler : IRequestHandler<UpsertBeerImageCommand, string>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService<Beer> _imagesService;

    /// <summary>
    ///     Initializes UpsertBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="imagesService">The images service</param>
    public UpsertBeerImageCommandHandler(IApplicationDbContext context, IImagesService<Beer> imagesService)
    {
        _context = context;
        _imagesService = imagesService;
    }

    /// <summary>
    ///     Handles UpsertBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<string> Handle(UpsertBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync(new object?[] { request.BeerId },
            cancellationToken: cancellationToken);
        if (beer == null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var entity = await _context.BeerImages.FirstOrDefaultAsync(x => x.BeerId == request.BeerId,
            cancellationToken: cancellationToken);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var imageUri = await _imagesService.UploadImageAsync(request.Image!, beer.BreweryId, request.BeerId);

            // Create new BeerImage for requested beer if it was not created for some reason.
            if (entity == null)
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