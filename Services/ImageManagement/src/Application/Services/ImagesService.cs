using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using SharedUtilities.Exceptions;

namespace Application.Services;

/// <summary>
///     Images service.
/// </summary>
public class ImagesService : IImagesService
{
    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IBlobStorageService _blobStorageService;

    /// <summary>
    ///     Initializes ImagesService.
    /// </summary>
    /// <param name="blobStorageService">The azure storage service</param>
    public ImagesService(IBlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="path">The image path</param>
    /// <param name="image">The image</param>
    public async Task<string> UploadImageAsync(string path, byte[] image)
    {
        var blobResponse = await _blobStorageService.UploadAsync(path, image);

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
        var blobResponse = await _blobStorageService.DeleteByUriAsync(imageUri);

        if (blobResponse.Error)
        {
            throw new RemoteServiceConnectionException(
                "Failed to delete the image.");
        }
    }
}