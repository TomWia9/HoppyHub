using Microsoft.AspNetCore.Http;

namespace SharedEvents;

/// <summary>
///     The beer image created event.
/// </summary>
public record BeerImageCreated
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }
    
    /// <summary>
    ///     The image path.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    ///     The beer image.
    /// </summary>
    public IFormFile? Image { get; init; }
}