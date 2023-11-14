using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using SharedEvents;
using SharedUtilities.Exceptions;

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
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes DeleteBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteBeerCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles DeleteBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Beers.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Beer), request.Id);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _context.Beers.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var imagesDeletedEvent = new ImagesDeleted
            {
                Paths = new List<string>
                {
                    $"Opinions/{entity.BreweryId}/{entity.Id}",
                    $"Beers/{entity.BreweryId}/{entity.Id}"
                }
            };

            // TODO: Ensure that event is successfully consumed by images microservice (Saga pattern?)
            //this should be consumed by images microservice which should delete all beer related images from blob container.
            await _publishEndpoint.Publish(imagesDeletedEvent, cancellationToken);
            //something like this:
            // await _azureStorageService.DeleteInPath(beerOpinionImagesPath);
            // await _azureStorageService.DeleteInPath(beerImagesPath);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}