using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

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
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes UpsertBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    public UpsertBeerImageCommandHandler(IApplicationDbContext context,
        IStorageContainerService storageContainerService)
    {
        _context = context;
        _storageContainerService = storageContainerService;
    }

    /// <summary>
    ///     Handles UpsertBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<string> Handle(UpsertBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync([request.BeerId], cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var fileName =
            $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}{Path.GetExtension(request.Image!.FileName)}";

        var imageUri = await _storageContainerService.UploadAsync(fileName, request.Image);

        if (string.IsNullOrEmpty(imageUri))
        {
            throw new RemoteServiceConnectionException("Failed to upload image.");
        }

        var entity = await _context.BeerImages.FirstOrDefaultAsync(x => x.BeerId == request.BeerId,
            cancellationToken);

        if (entity is null)
        {
            entity = new BeerImage
            {
                BeerId = beer.Id,
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

        await _context.SaveChangesAsync(CancellationToken.None);

        return imageUri;
    }
}