using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ImageManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ImageManagement.Functions;

/// <summary>
///     The DeleteImageFunction class.
/// </summary>
public abstract class DeleteImageFunction
{
    /// <summary>
    ///     Deletes image or images in given path from azure storage container.
    /// </summary>
    /// <param name="req">The http request</param>
    /// <returns>DeleteResult</returns>
    [Function("DeleteImage")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete")]
        HttpRequest req)
    {
        var path = req.Query["path"].ToString();

        if (string.IsNullOrEmpty(path))
        {
            return new BadRequestObjectResult("Path is empty");
        }

        path = path.Trim();
        var result = new DeleteResult();
        var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");
        var blobConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");

        if (string.IsNullOrEmpty(blobConnectionString) || string.IsNullOrEmpty(blobContainerName))
        {
            result.Success = false;
            result.ErrorMessage = "Storage account connection string or blob container name not found";

            return new BadRequestObjectResult(result);
        }

        try
        {
            var blobContainerClient = new BlobContainerClient(blobConnectionString, blobContainerName);

            foreach (var blobItem in blobContainerClient.GetBlobsByHierarchy(prefix: path))
            {
                if (blobItem.IsBlob)
                {
                    await blobContainerClient.DeleteBlobIfExistsAsync(blobItem.Blob.Name,
                        DeleteSnapshotsOption.IncludeSnapshots);
                }
            }
            
            result.Success = true;

            return new OkObjectResult(result);
        }
        catch (Exception e)
        {
            result.Success = false;
            result.ErrorMessage = $"Failed to delete files: {e.Message}";

            return new BadRequestObjectResult(result);
        }
    }
}