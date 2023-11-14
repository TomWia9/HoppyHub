namespace SharedEvents;

/// <summary>
///     The beer image deleted event.
/// </summary>
public record BeerImageDeleted
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }
    
    /// <summary>
    ///     The image path.
    /// </summary>
    public string? Path { get; init; }
}