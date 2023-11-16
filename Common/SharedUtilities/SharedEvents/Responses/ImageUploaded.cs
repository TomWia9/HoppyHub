namespace SharedEvents.Responses;

/// <summary>
///     The image uploaded event.
/// </summary>
public record ImageUploaded
{
    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? Uri { get; init; }
}