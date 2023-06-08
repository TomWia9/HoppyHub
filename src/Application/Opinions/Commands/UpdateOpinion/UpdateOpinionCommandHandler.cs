using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Opinions.Commands.UpdateOpinion;

/// <summary>
///     UpdateOpinionCommand handler.
/// </summary>
public class UpdateOpinionCommandHandler : IRequestHandler<UpdateOpinionCommand>
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
    ///     Initializes UpdateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="azureStorageService">The azure storage service</param>
    public UpdateOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IBeersService beersService, IAzureStorageService azureStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _beersService = beersService;
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    ///     Handles UpdateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateOpinionCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Opinions.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        if (!_currentUserService.AdministratorAccess &&
            entity.CreatedBy != _currentUserService.UserId)
        {
            throw new ForbiddenAccessException();
        }

        var imageUri = await HandleOpinionImageAsync(request.Image, entity.BeerId, entity.ImageUri);

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            entity.Rating = request.Rating;
            entity.Comment = request.Comment;
            entity.ImageUri = imageUri;

            await _context.SaveChangesAsync(cancellationToken);
            await _beersService.CalculateBeerRatingAsync(entity.BeerId);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    ///     Uploads image to blob container and returns image uri if request contains image, deletes it otherwise.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="entityImageUri">The current entity image uri</param>
    private async Task<string?> HandleOpinionImageAsync(IFormFile? image, Guid beerId, string? entityImageUri)
    {
        // If update request contains image then upload photo.
        if (image != null)
        {
            var path = CreateImagePath(image, beerId);

            var blobResponse = await _azureStorageService.UploadAsync(path, image);

            if (blobResponse.Error)
            {
                throw new RemoteServiceConnectionException("Failed to upload the image. The opinion was not saved.");
            }

            return blobResponse.Blob.Uri;
        }

        // If update request does not contain image and entity has image uri, then delete image from blob.
        if (!string.IsNullOrEmpty(entityImageUri))
        {
            var startIndex = entityImageUri.IndexOf("Opinions", StringComparison.Ordinal);
            var path = entityImageUri[startIndex..];

            var blobResponse = await _azureStorageService.DeleteAsync(path);

            if (blobResponse.Error)
            {
                throw new RemoteServiceConnectionException(
                    "Failed to delete the image. The opinion was not saved.");
            }

            return null;
        }

        return null;
    }

    /// <summary>
    ///     Returns file with name changed to match the folder structure in container "Opinions/BeerId/UserId.jpg/png"
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="beerId">The beer id</param>
    private string CreateImagePath(IFormFile file, Guid beerId)
    {
        var extension = Path.GetExtension(file.FileName);
        var userId = _currentUserService.UserId.ToString();

        return $"Opinions/{beerId.ToString()}/{userId}" + extension;
    }
}