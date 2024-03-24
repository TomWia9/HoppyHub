using System;
using System.Threading.Tasks;
using ImageManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace ImageManagement;

/// <summary>
///     The UploadBeerImage function class.
/// </summary>
public static class UploadBeerImage
{
    /// <summary>
    ///     Uploads beer image to azure storage container.
    /// </summary>
    /// <param name="req">The http request</param>
    /// <param name="logger">The logger</param>
    /// <returns>UploadResponse</returns>
    [FunctionName("UploadBeerImage")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req, ILogger logger)
    {
        var formData = await req.ReadFormAsync();
        var file = req.Form.Files["file"];
        var name = formData["name"];
        var image = new Image()
        {
            Name = name,
            File = file
        };
        var response = new UploadResponse();

        if (string.IsNullOrEmpty(image.Name) || image.File == null || image.File.Length == 0)
        {
            response.Success = false;
            response.ErrorMessage = "File not found";

            return new BadRequestObjectResult(response);
        }

        var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");
        var blobConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");

        if (!CloudStorageAccount.TryParse(blobConnectionString, out var storageAccount))
        {
            response.Success = false;
            response.ErrorMessage = "Invalid storage account connection string";

            return new BadRequestObjectResult(response);
        }

        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference(blobContainerName);

        await container.CreateIfNotExistsAsync();

        var blockBlob = container.GetBlockBlobReference(image.Name);

        await using (var stream = image.File.OpenReadStream())
        {
            await blockBlob.UploadFromStreamAsync(stream);
        }

        response.Success = true;
        response.Uri = blockBlob.Uri.ToString();

        return new OkObjectResult(blockBlob.Uri);
    }
}