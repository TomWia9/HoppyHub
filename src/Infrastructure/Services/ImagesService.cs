using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
///     Images service.
/// </summary>
public abstract class ImagesService : IImagesService
{
    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;
    
    /// <summary>
    ///     Initializes ImagesService.
    /// </summary>
    /// <param name="azureStorageService">The azure storage service</param>
    protected ImagesService(IAzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="path">The image path</param>
    /// <param name="image">The image</param>
    public async Task<string> UploadImageAsync(string path, IFormFile image)
    {
        var blobResponse = await _azureStorageService.UploadAsync(path, image);

        if (blobResponse.Error || string.IsNullOrEmpty(blobResponse.Blob.Uri))
        {
            throw new RemoteServiceConnectionException("Failed to upload the image.");
        }

        return blobResponse.Blob.Uri;
    }

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    public async Task DeleteImageAsync(string imageUri)
    {
        var blobResponse = await _azureStorageService.DeleteByUriAsync(imageUri);

        if (blobResponse.Error)
        {
            throw new RemoteServiceConnectionException(
                "Failed to delete the image.");
        }
    }
}