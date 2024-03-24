using Azure.Storage.Blobs;
using ImageManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ImageManagement;

/// <summary>
///     The UploadBeerImage function class.
/// </summary>
public abstract class UploadBeerImage
{
    /// <summary>
    ///     Uploads beer image to azure storage container.
    /// </summary>
    /// <param name="req">The http request</param>
    /// <returns>UploadResponse</returns>
    [Function("UploadBeerImage")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequest req)
    {
        var formData = await req.ReadFormAsync();
        var image = new Image
        {
            Name = formData["name"],
            File = req.Form.Files["file"]
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

        if (string.IsNullOrEmpty(blobConnectionString) || string.IsNullOrEmpty(blobContainerName))
        {
            response.Success = false;
            response.ErrorMessage = "Storage account connection string or blob container name not found";

            return new BadRequestObjectResult(response);
        }

        var blobContainerClient = new BlobContainerClient(blobConnectionString, blobContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(image.Name);

        await using (var stream = image.File.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        response.Success = true;
        response.Uri = blobClient.Uri.ToString();

        return new OkObjectResult(blobClient.Uri);
    }
}