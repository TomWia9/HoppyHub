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
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
            when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            _logger.LogError("File with name '{BlobFileName}' already exists in container", blob.FileName);
            response.Status =
                $"File with name {blob.FileName} already exists in container. Please use another name to store your file.";
            response.Error = true;
            return response;
        }
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // Log error to console and create a new response we can return to the requesting method
            _logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
            response.Error = true;
            return response;
        }

        // Return the BlobUploadResponse object
        return response;
    }

    /// <summary>
    ///     Downloads a blob with the specified filename.
    /// </summary>
    /// <param name="blobFilename">Filename</param>
    /// <returns>Blob</returns>
    public async Task<BlobDto> DownloadAsync(string blobFilename)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Deletes a blob with the specified filename.
    /// </summary>
    /// <param name="blobFilename">Filename</param>
    /// <returns>BlobResponseDto with status</returns>
    public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
    {
        throw new NotImplementedException();
    }
}