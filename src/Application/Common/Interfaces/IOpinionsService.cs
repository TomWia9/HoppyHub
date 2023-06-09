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
    /// <param name="beerId">The beer id</param>
    Task<string?> HandleOpinionImageUploadAsync(IFormFile image, Guid beerId);

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    Task HandleOpinionImageDeleteAsync(string imageUri);
}