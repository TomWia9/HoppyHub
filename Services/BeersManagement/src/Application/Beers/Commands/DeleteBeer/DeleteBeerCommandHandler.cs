using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using SharedEvents.Events;
using SharedEvents.Responses;
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
    ///     The ImagesDeleted request client.
    /// </summary>
    private readonly IRequestClient<ImagesDeleted> _imagesDeletedRequestClient;

    /// <summary>
    ///     Initializes DeleteBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="imagesDeletedRequestClient">The ImagesDeleted request client</param>
    public DeleteBeerCommandHandler(IApplicationDbContext context,
        IRequestClient<ImagesDeleted> imagesDeletedRequestClient)
    {
        _context = context;
        _imagesDeletedRequestClient = imagesDeletedRequestClient;
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
            var result =
                await _imagesDeletedRequestClient.GetResponse<ImagesDeletedFromBlobStorage>(imagesDeletedEvent,
                    cancellationToken);
            var imagesDeletedSuccessfully = result.Message.Success;

            if (!imagesDeletedSuccessfully)
            {
                throw new RemoteServiceConnectionException("There was a problem deleting the images.");
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}