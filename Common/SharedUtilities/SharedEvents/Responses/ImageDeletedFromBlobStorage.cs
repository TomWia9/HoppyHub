namespace SharedEvents.Responses;

/// <summary>
///     The image deleted from blob storage event.
/// </summary>
public record ImageDeletedFromBlobStorage
{
    /// <summary>
    ///     Indicates whether operation succeed.
    /// </summary>
    public bool Success { get; init; }
}