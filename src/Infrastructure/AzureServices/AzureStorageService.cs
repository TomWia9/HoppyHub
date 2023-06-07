﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models.BlobContainer;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.AzureServices;

/// <summary>
///     The AzureStorageService service.
/// </summary>
public class AzureStorageService : IAzureStorageService
{
    /// <summary>
    ///     The logger.
    /// </summary>
    private readonly ILogger<AzureStorageService> _logger;

    /// <summary>
    ///     The blob container client.
    /// </summary>
    private readonly BlobContainerClient _blobContainerClient;

    /// <summary>
    ///     Initializes AzureStorageService.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="configuration">The configuration</param>
    public AzureStorageService(ILogger<AzureStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _blobContainerClient = CreateBlobContainerClient(configuration);
    }

    /// <summary>
    ///     Creates blob container client.
    /// </summary>
    /// <param name="configuration">The configuration</param>
    private BlobContainerClient CreateBlobContainerClient(IConfiguration configuration)
    {
        var blobConnectionString = configuration.GetValue<string>("BlobContainerSettings:BlobConnectionString");
        var blobContainerName = configuration.GetValue<string>("BlobContainerSettings:BlobContainerName");

        if (string.IsNullOrEmpty(blobConnectionString))
        {
            throw new RemoteServiceConnectionException("The blob storage connection string is null");
        }

        if (string.IsNullOrEmpty(blobContainerName))
        {
            throw new RemoteServiceConnectionException("The blob storage container name is null");
        }

        try
        {
            return new BlobContainerClient(blobConnectionString, blobContainerName);
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot connect to te blob container. Exception message: {ExMessage}", e.Message);
            throw new RemoteServiceConnectionException(e.Message);
        }
    }

    /// <summary>
    ///     Uploads a file submitted with the request.
    /// </summary>
    /// <param name="blob">The blob for upload</param>
    /// <returns>BlobResponseDto with status</returns>
    public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
    {
        BlobResponseDto response = new();

        try
        {
            var client = _blobContainerClient.GetBlobClient(blob.FileName);

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
    ///     Deletes a blob with the specified filename.
    /// </summary>
    /// <param name="blobFilename">Filename</param>
    /// <returns>BlobResponseDto with status</returns>
    public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
    {
        var file = _blobContainerClient.GetBlobClient(blobFilename);

        try
        {
            await file.DeleteAsync();
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            _logger.LogError("File {BlobFilename} was not found", blobFilename);
            return new BlobResponseDto { Error = true, Status = $"File with name {blobFilename} not found." };
        }

        return new BlobResponseDto { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };
    }
}