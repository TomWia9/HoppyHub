using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

/// <summary>
///     The beers images service.
/// </summary>
public class BeersImagesService : ImagesService, IBeersImagesService
{
    /// <summary>
    ///     The configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Initializes BeersImagesService.
    /// </summary>
    /// <param name="azureStorageService">The azure storage service</param>
    /// <param name="configuration">The configuration</param>
    public BeersImagesService(IAzureStorageService azureStorageService, IConfiguration configuration) : base(
        azureStorageService)
    {
        _configuration = configuration;
    }

    /// <summary>
    ///     Gets beer temp image uri.
    /// </summary>
    public string GetTempBeerImageUri()
    {
        return _configuration.GetValue<string>("TempBeerImageUri") ??
               throw new InvalidOperationException("Temp beer image uri does not exists.");
    }

    /// <summary>
    ///     Returns beer image path matching the folder structure in container.
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    public string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId)
    {
        var extension = Path.GetExtension(file.FileName);

        return $"Beers/{breweryId.ToString()}/{beerId.ToString()}" + extension;
    }
}