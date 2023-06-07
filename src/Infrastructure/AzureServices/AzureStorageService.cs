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

        var blobConnectionString = configuration.GetValue<string>("BlobContainerSettings:BlobConnectionString");
        var blobContainerName = configuration.GetValue<string>("BlobContainerSettings:BlobContainerName");

        _blobContainerClient = new BlobContainerClient(blobConnectionString, blobContainerName);
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
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError("File with name '{BlobFileName}' already exists in container", blob.FileName);

            response.Status =
                $"File with name {blob.FileName} already exists in container. Please use another name to store your file.";
            response.Error = true;

            return response;
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
    ///     Downloads a blob with the specified filename.
    /// </summary>
    /// <param name="blobFilename">Filename</param>
    /// <returns>Blob</returns>
    public async Task<BlobDto?> DownloadAsync(string blobFilename)
    {
        try
        {
            var file = _blobContainerClient.GetBlobClient(blobFilename);

            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                var content = await file.DownloadContentAsync();
                var contentType = content.Value.Details.ContentType;

                return new BlobDto { Content = data, Name = blobFilename, ContentType = contentType };
            }
        }
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            _logger.LogError("File {BlobFilename} was not found", blobFilename);
        }

        return null;
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