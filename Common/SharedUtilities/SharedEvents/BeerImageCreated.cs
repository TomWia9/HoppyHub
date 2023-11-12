using Microsoft.AspNetCore.Http;

namespace SharedEvents;

/// <summary>
///     The beer created event.
/// </summary>
public record BeerImageCreated
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }
    
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid BreweryId { get; init; }

    /// <summary>
    ///     The beer image.
    /// </summary>
    public IFormFile? Image { get; init; }
}