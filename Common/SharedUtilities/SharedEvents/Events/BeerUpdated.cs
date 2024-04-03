namespace SharedEvents.Events;

/// <summary>
///     The beer updated event.
/// </summary>
public record BeerUpdated
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The beer brewery id.
    /// </summary>
    public Guid BreweryId { get; set; }
}