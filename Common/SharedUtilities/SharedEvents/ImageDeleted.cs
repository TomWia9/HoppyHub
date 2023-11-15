namespace SharedEvents;

/// <summary>
///     The image deleted event.
/// </summary>
public record ImageDeleted
{
    /// <summary>
    ///     The uri of image.
    /// </summary>
    public string? Uri { get; set; }
}