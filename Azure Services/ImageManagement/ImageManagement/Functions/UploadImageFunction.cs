using Azure.Storage.Blobs;
using ImageManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ImageManagement.Functions;

/// <summary>
///     The UploadImageFunction class.
/// </summary>
public abstract class UploadImageFunction
{
    /// <summary>
    ///     Uploads image to azure storage container.
    /// </summary>
    /// <param name="req">The http request</param>
    /// <returns>UploadResult</returns>
    [Function("UploadImage")]
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
        var result = new UploadResult();

        if (string.IsNullOrEmpty(image.Name) || image.File == null || image.File.Length == 0)
        {
            result.Success = false;
            result.ErrorMessage = "File not found";

            return new BadRequestObjectResult(result);
        }

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
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(image.Name);

            await using (var stream = image.File.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            result.Success = true;
            result.Uri = blobClient.Uri.ToString();

            return new OkObjectResult(result);
        }
        catch (Exception e)
        {
            result.Success = false;
            result.ErrorMessage = $"Failed to upload file: {e.Message}";

            return new BadRequestObjectResult(result);
        }
    }
}