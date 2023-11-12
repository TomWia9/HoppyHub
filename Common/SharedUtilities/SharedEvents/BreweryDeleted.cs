namespace SharedEvents;

/// <summary>
///     The brewery deleted event.
/// </summary>
public record BreweryDeleted
{
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid Id { get; set; }
}