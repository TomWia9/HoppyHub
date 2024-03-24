using Microsoft.AspNetCore.Http;

namespace ImageManagement.Models;

/// <summary>
///     The image model.
/// </summary>
public record Image
{
    /// <summary>
    ///     The image name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The beer image.
    /// </summary>
    public IFormFile? File { get; init; }
}