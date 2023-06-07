using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces;
using Application.Common.Models.BlobContainer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Test purpose only.
/// </summary>
[ExcludeFromCodeCoverage]
[Route("api/[controller]")]
[ApiController]
public class TestStorageController : ControllerBase
{
    private readonly IAzureStorageService _azureStorageService;

    public TestStorageController(IAzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var response = await _azureStorageService.UploadAsync(file);

        return response.Error
            ? StatusCode(StatusCodes.Status500InternalServerError, response.Status)
            : StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpGet("{filename}")]
    public async Task<IActionResult> Download(string filename)
    {
        var file = await _azureStorageService.DownloadAsync("test/test/" + filename);

        return File(file.Content, file.ContentType, file.Name);
    }

    [HttpDelete("{filename}")]
    public async Task<IActionResult> Delete(string filename)
    {
        var response = await _azureStorageService.DeleteAsync("test/test/" + filename);

        return StatusCode(response.Error ? StatusCodes.Status500InternalServerError : StatusCodes.Status200OK,
            response.Status);
    }
}