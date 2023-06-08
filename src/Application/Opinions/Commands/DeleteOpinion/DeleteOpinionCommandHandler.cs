using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Opinions.Commands.DeleteOpinion;

/// <summary>
///     DeleteOpinionCommand handler
/// </summary>
public class DeleteOpinionCommandHandler : IRequestHandler<DeleteOpinionCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     Initializes DeleteOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="azureStorageService">The azure storage service</param>
    public DeleteOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IBeersService beersService, IAzureStorageService azureStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _beersService = beersService;
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    ///     Handles DeleteOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteOpinionCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Opinions.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        if (!_currentUserService.AdministratorAccess &&
            entity.CreatedBy != _currentUserService.UserId)
        {
            throw new ForbiddenAccessException();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            _context.Opinions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            await _beersService.CalculateBeerRatingAsync(entity.BeerId);
            await _context.SaveChangesAsync(cancellationToken);
            await HandleOpinionImageDeleteAsync(entity.ImageUri); //TODO check if this is null
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    /// <summary>
    ///     Deletes image from blob if opinion has image.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    private async Task HandleOpinionImageDeleteAsync(string? imageUri)
    {
        if (!string.IsNullOrEmpty(imageUri))
        {
            var startIndex = imageUri.IndexOf("Opinions", StringComparison.Ordinal);
            var path = imageUri[startIndex..];

            var blobResponse = await _azureStorageService.DeleteAsync(path);

            if (blobResponse.Error)
            {
                throw new RemoteServiceConnectionException(
                    "Failed to delete the image. The opinion was not deleted.");
            }
        }
    }
}