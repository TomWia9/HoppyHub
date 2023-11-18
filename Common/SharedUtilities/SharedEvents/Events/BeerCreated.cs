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
    
    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The beer brewery name.
    /// </summary>
    public string? BreweryName { get; set; }
}