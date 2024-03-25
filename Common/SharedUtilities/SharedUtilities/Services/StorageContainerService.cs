using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace SharedUtilities.Services;

/// <summary>
///     StorageContainer service.
/// </summary>
public class StorageContainerService : IStorageContainerService
{
    /// <summary>
    ///     The blob container client.
    /// </summary>
    private readonly BlobContainerClient _blobContainerClient;

    /// <summary>
    ///     Initializes StorageContainerService.
    /// </summary>
    public StorageContainerService(string? storageAccountConnectionString, string? blobContainerName)
    {
        if (string.IsNullOrEmpty(storageAccountConnectionString))
        {
            throw new ArgumentNullException(nameof(storageAccountConnectionString));
        }

        if (string.IsNullOrEmpty(blobContainerName))
        {
            throw new ArgumentNullException(nameof(blobContainerName));
        }

        var blobServiceClient = new BlobServiceClient(storageAccountConnectionString);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
    }

    /// <summary>
    ///     Uploads file async.
    /// </summary>
    /// <param name="fileName">The file</param>
    /// <param name="file">The file name</param>
    /// <returns>File uri</returns>
    public async Task<string> UploadAsync(string fileName, IFormFile file)
    {
        try
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }
        catch
        {
            throw new RemoteServiceConnectionException("Failed to upload file");
        }
    }

    /// <summary>
    ///     Deletes files from given path.
    /// </summary>
    /// <param name="path">The path</param>
    public async Task DeleteFromPathAsync(string path)
    {
        try
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
        catch
        {
            throw new RemoteServiceConnectionException("Failed to delete");
        }
    }
}