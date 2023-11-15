using Microsoft.AspNetCore.Http;

namespace SharedEvents;

/// <summary>
///     The image created event.
/// </summary>
public record ImageCreated
{
    /// <summary>
    ///     The image path.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    ///     The beer image.
    /// </summary>
    public byte[]? Image { get; init; }
}