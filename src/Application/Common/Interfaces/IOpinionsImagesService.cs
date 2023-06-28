using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     The opinions images service interface.
/// </summary>
public interface IOpinionsImagesService : IImagesService
{
    /// <summary>
    ///     Returns opinion image path matching the folder structure in container.
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId, Guid opinionId);
}