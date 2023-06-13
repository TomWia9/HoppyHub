using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     ImagesService interface.
/// </summary>
public interface IImagesService<T>
{
    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    Task<string> UploadImageAsync(IFormFile image, Guid breweryId, Guid beerId, Guid? opinionId = null);

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    Task DeleteImageAsync(string imageUri);

    /// <summary>
    ///     Gets temp image uri.
    /// </summary>
    string GetTempImageUri();
}