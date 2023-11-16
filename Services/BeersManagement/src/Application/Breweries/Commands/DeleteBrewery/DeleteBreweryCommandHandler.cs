using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

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
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes DeleteBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteBreweryCommandHandler(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
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
            await _context.Breweries.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Brewery), request.Id);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _context.Breweries.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var imagesDeletedEvent = new ImagesDeleted
            {
                Paths = new List<string>
                {
                    $"Opinions/{entity.Id}",
                    $"Beers/{entity.Id}"
                }
            };

            // TODO: Ensure that event is successfully consumed by images microservice (Saga pattern?)
            //this should be consumed by images microservice which should delete all brewery related images from blob container.
            await _publishEndpoint.Publish(imagesDeletedEvent, cancellationToken);
            //something like this:
            // await _azureStorageService.DeleteInPath(breweryBeersOpinionImagesPath);
            // await _azureStorageService.DeleteInPath(breweryBeerImagesPath);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}