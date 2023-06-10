using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Opinions.Services;

/// <summary>
///     Opinions service.
/// </summary>
public class OpinionsService : IOpinionsService
{
    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     Initializes OpinionsService.
    /// </summary>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="azureStorageService">The azure storage service</param>
    public OpinionsService(ICurrentUserService currentUserService, IAzureStorageService azureStorageService)
    {
        _currentUserService = currentUserService;
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    public async Task<string?> UploadOpinionImageAsync(IFormFile image, Guid breweryId, Guid beerId)
    {
        var path = CreateImagePath(image, breweryId, beerId);
        var blobResponse = await _azureStorageService.UploadAsync(path, image);

        if (blobResponse.Error)
        {
            throw new RemoteServiceConnectionException("Failed to upload the image. The opinion was not saved.");
        }

        return blobResponse.Blob.Uri;
    }

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    public async Task DeleteOpinionImageAsync(string imageUri)
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

    /// <summary>
    ///     Returns image path to match the folder structure in container "Opinions/BreweryId/BeerId/UserId.jpg/png"
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    private string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId)
    {
        var extension = Path.GetExtension(file.FileName);
        var userId = _currentUserService.UserId.ToString();

        return $"Opinions/{breweryId.ToString()}/{beerId.ToString()}/{userId}" + extension;
    }
}