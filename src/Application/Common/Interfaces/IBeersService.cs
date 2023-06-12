using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     BeersService interface.
/// </summary>
public interface IBeersService
{
    /// <summary>
    ///     Calculates beer average rating asynchronously.
    /// </summary>
    /// <param name="beerId">The beer id</param>
    Task CalculateBeerRatingAsync(Guid beerId);
    
    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    Task<string> UploadBeerImageAsync(IFormFile image, Guid breweryId, Guid beerId);

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    Task DeleteBeerImageAsync(string imageUri);

    /// <summary>
    ///     Gets temp image uri.
    /// </summary>
    string GetTempImageUri();
}