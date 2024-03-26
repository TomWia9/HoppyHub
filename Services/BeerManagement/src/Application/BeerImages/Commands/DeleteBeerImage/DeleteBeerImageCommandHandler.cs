using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     DeleteBeerImageCommand handler.
/// </summary>
public class DeleteBeerImageCommandHandler : IRequestHandler<DeleteBeerImageCommand>
{
    /// <summary>
    ///     The app configuration.
    /// </summary>
    private readonly IAppConfiguration _appConfiguration;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes DeleteBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    /// <param name="appConfiguration">The app configuration</param>
    public DeleteBeerImageCommandHandler(IApplicationDbContext context,
        IStorageContainerService storageContainerService,
        IAppConfiguration appConfiguration)
    {
        _context = context;
        _storageContainerService = storageContainerService;
        _appConfiguration = appConfiguration;
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
            var path = $"Beers/{beer.BreweryId}/{beer.Id}{Path.GetExtension(beer.BeerImage.ImageUri)}";
            
            await _storageContainerService.DeleteFromPathAsync(path);

            beer.BeerImage.ImageUri = _appConfiguration.TempBeerImageUri;
            beer.BeerImage.TempImage = true;

            await _context.SaveChangesAsync(CancellationToken.None);
        }
        else
        {
            throw new BadRequestException("Image already deleted.");
        }
    }
}