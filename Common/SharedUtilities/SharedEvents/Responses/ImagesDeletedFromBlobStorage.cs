namespace SharedEvents.Responses;

/// <summary>
///     The images deleted from blob storage event.
/// </summary>
public record ImagesDeletedFromBlobStorage
{
    /// <summary>
    ///     Indicates whether operation succeed.
    /// </summary>
    public bool Success { get; init; }
}