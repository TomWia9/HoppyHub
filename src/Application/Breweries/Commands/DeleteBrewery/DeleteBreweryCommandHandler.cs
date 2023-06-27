﻿using Application.Common.Exceptions;
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
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

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

            //Delete all brewery related images from blob container.
            var breweryBeersOpinionImagesPath = $"Opinions/{entity.Id}";
            var breweryBeerImagesPath = $"Beers/{entity.Id}";

            await _azureStorageService.DeleteFilesInPath(breweryBeersOpinionImagesPath);
            await _azureStorageService.DeleteFilesInPath(breweryBeerImagesPath);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}