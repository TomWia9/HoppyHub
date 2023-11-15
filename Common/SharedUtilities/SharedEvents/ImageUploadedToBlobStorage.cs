namespace SharedEvents;

/// <summary>
///     The image uploaded to blob storage event.
/// </summary>
public record ImageUploadedToBlobStorage
{
    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; init; }
}