namespace SharedEvents;

/// <summary>
///     The beer image deleted event.
/// </summary>
public record BeerImageDeleted
{
    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; init; }
}