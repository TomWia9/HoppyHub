using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

/// <summary>
///     The opinions images service.
/// </summary>
public class OpinionsImagesService : ImagesService, IOpinionsImagesService
{
    /// <summary>
    ///     Initializes OpinionsImagesService.
    /// </summary>
    /// <param name="azureStorageService">The azure storage service</param>
    public OpinionsImagesService(IAzureStorageService azureStorageService) : base(azureStorageService)
    {
    }

    /// <summary>
    ///     Returns opinion image path matching the folder structure in container.
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    public string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId, Guid opinionId)
    {
        var extension = Path.GetExtension(file.FileName);

        return $"Opinions/{breweryId.ToString()}/{beerId.ToString()}/{opinionId.ToString()}" + extension;
    }
}