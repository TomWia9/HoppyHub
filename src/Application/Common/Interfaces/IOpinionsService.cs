using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     OpinionsService interface.
/// </summary>
public interface IOpinionsService
{
    /// <summary>
    ///     Uploads image to blob container and returns image uri if request contains image.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    Task<string?> UploadOpinionImageAsync(IFormFile image, Guid breweryId, Guid beerId);

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    Task DeleteOpinionImageAsync(string imageUri);
}