using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

/// <summary>
///     The images service interface.
/// </summary>
public interface IImagesService
{
    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="path">The image path</param>
    /// <param name="image">The image</param>
    Task<string> UploadImageAsync(string path, byte[] image);

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    Task DeleteImageAsync(string imageUri);
}