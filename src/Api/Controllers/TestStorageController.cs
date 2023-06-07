using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Test purpose only.
/// </summary>
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
}