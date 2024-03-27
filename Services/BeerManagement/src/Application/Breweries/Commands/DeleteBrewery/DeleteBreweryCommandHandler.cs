using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using SharedEvents.Events;
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
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes DeleteBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteBreweryCommandHandler(IApplicationDbContext context,
        IStorageContainerService storageContainerService, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _storageContainerService = storageContainerService;
        _publishEndpoint = publishEndpoint;
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

            await _storageContainerService.DeleteFromPathAsync(breweryBeersImagesPath);

            await transaction.CommitAsync(cancellationToken);

            var breweryDeletedEvent = new BreweryDeleted
            {
                Id = entity.Id
            };

            await _publishEndpoint.Publish(breweryDeletedEvent, cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}