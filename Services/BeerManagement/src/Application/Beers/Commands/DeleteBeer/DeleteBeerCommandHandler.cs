using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.Beers.Commands.DeleteBeer;

/// <summary>
///     DeleteBeerCommand handler
/// </summary>
public class DeleteBeerCommandHandler : IRequestHandler<DeleteBeerCommand>
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
    ///     Initializes DeleteBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="storageContainerService">The storage container service</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteBeerCommandHandler(IApplicationDbContext context,
        IStorageContainerService storageContainerService,
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _storageContainerService = storageContainerService;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles DeleteBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Beers.FindAsync([request.Id], cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Beer), request.Id);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _context.Beers.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var beerImagePath = $"Beers/{entity.BreweryId}/{entity.Id}";
            await _storageContainerService.DeleteFromPathAsync(beerImagePath);

            await transaction.CommitAsync(cancellationToken);

            var beerDeletedEvent = new BeerDeleted
            {
                Id = entity.Id
            };

            await _publishEndpoint.Publish(beerDeletedEvent, cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}