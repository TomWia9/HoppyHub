using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

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
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes DeleteBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    public DeleteBreweryCommandHandler(IApplicationDbContext context,
        IStorageContainerService storageContainerService)
    {
        _context = context;
        _storageContainerService = storageContainerService;
    }

    /// <summary>
    ///     Handles DeleteBreweryCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBreweryCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Breweries.FindAsync([request.Id], cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Brewery), request.Id);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _context.Breweries.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var breweryBeersImagesPath = $"Beers/{entity.Id}";
            var breweryBeerOpinionsImagesPath = $"Opinions/{entity.Id}";

            await _storageContainerService.DeleteFromPathAsync(breweryBeersImagesPath);
            await _storageContainerService.DeleteFromPathAsync(breweryBeerOpinionsImagesPath);

            await transaction.CommitAsync(cancellationToken);

            //TODO: Create and send this event and handle in OpinionManagement and FavoriteManagement
            // var breweryDeletedEvent = new BreweryDeleted
            // {
            //     Id = entity.Id
            // };
            //
            // await _publishEndpoint.Publish(breweryDeletedEvent, cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}