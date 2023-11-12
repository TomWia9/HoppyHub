namespace SharedEvents;

/// <summary>
///     The beer image uploaded to blob storage event.
/// </summary>
public record BeerImageUploadedToBlobStorage
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