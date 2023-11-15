namespace SharedEvents;

/// <summary>
///     The images deleted event.
/// </summary>
public record ImagesDeleted
{
    /// <summary>
    ///     The list of path from which to delete images.
    /// </summary>
    public IEnumerable<string>? Paths { get; set; }
}