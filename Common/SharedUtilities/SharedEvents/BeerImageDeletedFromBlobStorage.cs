namespace SharedEvents;

/// <summary>
///     The beer image deleted from blob storage event.
/// </summary>
public record BeerImageDeletedFromBlobStorage
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }
}