﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Services;

/// <summary>
///     Images service.
/// </summary>
public class ImagesService<T> : IImagesService<T>
{
    /// <summary>
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     Temp image uri.
    /// </summary>
    private const string TempImageUri = "https://hoppyhub.blob.core.windows.net/hoppyhub-container/Beers/temp.jpg";

    /// <summary>
    ///     The entity name.
    /// </summary>
    private readonly string _entityName;

    /// <summary>
    ///     The entity names dictionary.
    /// </summary>
    private readonly Dictionary<Type, string> _entityNamesDictionary = new()
    {
        { typeof(Beer), "Beers" },
        { typeof(Opinion), "Opinions" }
    };

    /// <summary>
    ///     Initializes ImagesService.
    /// </summary>
    /// <param name="azureStorageService">The azure storage service</param>
    public ImagesService(IAzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;

        if (_entityNamesDictionary.TryGetValue(typeof(T), out var entityName))
        {
            _entityName = entityName;
        }
        else
        {
            throw new KeyNotFoundException($"Entity name not found for type {typeof(T)}.");
        }
    }

    /// <summary>
    ///     Uploads image to blob container and returns image uri.
    /// </summary>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    public async Task<string> UploadImageAsync(IFormFile image, Guid breweryId, Guid beerId, Guid? opinionId = null)
    {
        var path = CreateImagePath(image, breweryId, beerId, opinionId);
        var blobResponse = await _azureStorageService.UploadAsync(path, image);

        if (blobResponse.Error || string.IsNullOrEmpty(blobResponse.Blob.Uri))
        {
            throw new RemoteServiceConnectionException("Failed to upload the image.");
        }

        return blobResponse.Blob.Uri;
    }

    /// <summary>
    ///     Deletes image from blob.
    /// </summary>
    /// <param name="imageUri">The image uri</param>
    public async Task DeleteImageAsync(string imageUri)
    {
        var startIndex = imageUri.IndexOf(_entityName, StringComparison.Ordinal);
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
    ///     Returns image path to match the folder structure in container.
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    private string CreateImagePath(IFormFile file, Guid breweryId, Guid beerId, Guid? opinionId)
    {
        var extension = Path.GetExtension(file.FileName);

        if (typeof(T) == typeof(Beer))
        {
            return $"Beers/{breweryId.ToString()}/{beerId.ToString()}" + extension;
        }
        
        return $"Opinions/{breweryId.ToString()}/{beerId.ToString()}/{opinionId.ToString()}" + extension;
    }
}