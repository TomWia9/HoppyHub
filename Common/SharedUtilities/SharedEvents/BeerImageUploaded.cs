namespace SharedEvents;

/// <summary>
///     The beer image uploaded event.
/// </summary>
public record BeerImageUploaded
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }
    
    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; init; }
}