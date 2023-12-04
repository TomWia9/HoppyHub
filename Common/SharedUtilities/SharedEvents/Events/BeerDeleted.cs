namespace SharedEvents.Events;

/// <summary>
///     The beer deleted event.
/// </summary>
public record BeerDeleted
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid Id { get; set; }
}