using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     The beers images service interface.
/// </summary>
public interface IBeersImagesService : IImagesService
{
    /// <summary>
    ///     Gets beer temp image uri.
    /// </summary>
    string GetTempBeerImageUri();

    /// <summary>
    ///     Returns beer image path matching the folder structure in container.
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId);
}