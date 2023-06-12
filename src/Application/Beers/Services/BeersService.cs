using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Beers.Services;

/// <summary>
///     Beers service.
/// </summary>
public class BeersService : IBeersService
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     Temp image uri.
    /// </summary>
    private const string TempImageUri = "https://hoppyhub.blob.core.windows.net/hoppyhub-container/Beers/temp.jpg";

    /// <summary>
    ///     Initializes BeersService.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="azureStorageService">The azure storage service</param>
    public BeersService(IApplicationDbContext context, IAzureStorageService azureStorageService)
    {
        _context = context;
        _azureStorageService = azureStorageService;
    }

    /// <summary>
    ///     Calculates beer average rating.
    /// </summary>
    public async Task CalculateBeerRatingAsync(Guid beerId)
    {
        var beer = await _context.Beers.FindAsync(beerId);

        if (beer == null)
        {
            throw new NotFoundException(nameof(Beer), beerId);
        }

        var beerRating = await _context.Opinions.Where(x => x.BeerId == beerId)
            .AverageAsync(x => x.Rating);

        beer.Rating = Math.Round(beerRating, 2);
    }

    public async Task<string> UploadBeerImageAsync(IFormFile image, Guid breweryId, Guid beerId)
    {
        var path = CreateImagePath(image, breweryId, beerId);
        var blobResponse = await _azureStorageService.UploadAsync(path, image);

        if (blobResponse.Error || string.IsNullOrEmpty(blobResponse.Blob.Uri))
        {
            throw new RemoteServiceConnectionException("Failed to upload the image.");
        }

        return blobResponse.Blob.Uri;
    }

    public async Task DeleteBeerImageAsync(string imageUri)
    {
        var startIndex = imageUri.IndexOf("Beers", StringComparison.Ordinal);
        var path = imageUri[startIndex..];

        var blobResponse = await _azureStorageService.DeleteAsync(path);

        if (blobResponse.Error)
        {
            throw new RemoteServiceConnectionException(
                "Failed to delete the image.");
        }
    }

    /// <summary>
    ///     Gets temp image uri.
    /// </summary>
    public string GetTempImageUri()
    {
        return TempImageUri;
    }

    /// <summary>
    ///     Returns image path to match the folder structure in container "Beers/BreweryId/BeerId.jpg/png"
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    private static string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId)
    {
        var extension = Path.GetExtension(file.FileName);

        return $"Beers/{breweryId.ToString()}/{beerId.ToString()}" + extension;
    }
}