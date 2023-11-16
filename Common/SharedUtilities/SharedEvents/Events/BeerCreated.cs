namespace SharedEvents.Events;

/// <summary>
///     The beer created event.
/// </summary>
public record BeerCreated
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid Id { get; set; }
}