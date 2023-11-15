﻿using Application.Interfaces;
using Application.Models.BlobContainer;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
///     The BlobStorageService service.
/// </summary>
public class BlobStorageService : IBlobStorageService
{
    /// <summary>
    ///     The blob container client.
    /// </summary>
    private readonly BlobContainerClient _blobContainerClient;

    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<BlobStorageService> _logger;

    /// <summary>
    ///     Initializes AzureStorageService.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="blobContainerClient">The blob container client</param>
    public BlobStorageService(ILogger<BlobStorageService> logger, BlobContainerClient blobContainerClient)
    {
        _logger = logger;
        _blobContainerClient = blobContainerClient;
    }

    /// <summary>
    ///     Uploads a file submitted with the request.
    /// </summary>
    /// <param name="path">The blob path</param>
    /// <param name="blob">The blob for upload</param>
    /// <returns>BlobResponseDto with status</returns>
    public async Task<BlobResponseDto> UploadAsync(string path, IFormFile blob)
    {
        BlobResponseDto response = new();

        try
        {
            var client = _blobContainerClient.GetBlobClient(path);

            await using (var data = blob.OpenReadStream())
            {
                await client.UploadAsync(data, true);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Unhandled Exception. ID: {ExStackTrace} - Message: {ExMessage}", ex.StackTrace,
                ex.Message);

            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;

            return response;
        }

        return response;
    }

    /// <summary>
    ///     Deletes a blob on a given path.
    /// </summary>
    /// <param name="path">The blob path</param>
    /// <returns>BlobResponseDto with status</returns>
    public async Task<BlobResponseDto> DeleteAsync(string path)
    {
        var file = _blobContainerClient.GetBlobClient(path);

        try
        {
            await file.DeleteAsync();
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            _logger.LogError("File {Path} was not found", path);
            return new BlobResponseDto { Error = true, Status = $"File with name {path} not found." };
        }

        return new BlobResponseDto { Error = false, Status = $"File: {path} has been successfully deleted." };
    }

    /// <summary>
    ///     Deletes a blob by blob uri.
    /// </summary>
    /// <param name="uri">The blob uri</param>
    /// <returns>BlobResponseDto with status</returns>
    public async Task<BlobResponseDto> DeleteByUriAsync(string uri)
    {
        var uriBuilder = new BlobUriBuilder(new Uri(uri));
        var path = uriBuilder.BlobName;

        return await DeleteAsync(path);
    }

    /// <summary>
    ///     Deletes all blobs in given path.
    /// </summary>
    /// <param name="path">The path</param>
    public async Task DeleteInPath(string path)
    {
        foreach (var blobItem in _blobContainerClient.GetBlobsByHierarchy(prefix: path))
        {
            if (blobItem.IsBlob)
            {
                await _blobContainerClient.DeleteBlobIfExistsAsync(blobItem.Blob.Name,
                    DeleteSnapshotsOption.IncludeSnapshots);
            }
        }
    }
}