using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using SharedUtilities.Exceptions;

namespace Application.Beers.Commands.DeleteBeer;

/// <summary>
///     DeleteBeerCommand handler
/// </summary>
public class DeleteBeerCommandHandler : IRequestHandler<DeleteBeerCommand>
{
    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes DeleteBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="azureStorageService">The azure storage service</param>
    public DeleteBeerCommandHandler(IApplicationDbContext context, IAzureStorageService azureStorageService)
    {
        _context = context;
        _azureStorageService = azureStorageService;
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

            //Delete all beer related images from blob container.
            var beerOpinionImagesPath = $"Opinions/{entity.BreweryId}/{entity.Id}";
            var beerImagesPath = $"Beers/{entity.BreweryId}/{entity.Id}";

            await _azureStorageService.DeleteInPath(beerOpinionImagesPath);
            await _azureStorageService.DeleteInPath(beerImagesPath);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}