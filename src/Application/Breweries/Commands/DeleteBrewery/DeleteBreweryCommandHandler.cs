using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

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
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     Initializes DeleteBreweryCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="azureStorageService">The azure storage service</param>
    public DeleteBreweryCommandHandler(IApplicationDbContext context, IAzureStorageService azureStorageService)
    {
        _context = context;
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    ///     Handles DeleteBreweryCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBreweryCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Breweries.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Brewery), request.Id);
        }

        // TODO: Wrap this in transaction
        _context.Breweries.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        //Delete all beer opinions.
        var breweryBeersOpinionImagesPath = $"Opinions/{entity.Id}";

        await _azureStorageService.DeleteFilesInPath(breweryBeersOpinionImagesPath);
        //transaction commit.
    }
}