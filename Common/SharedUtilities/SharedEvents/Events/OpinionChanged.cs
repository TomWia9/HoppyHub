namespace SharedEvents.Events;

/// <summary>
///     The opinion changed event.
/// </summary>
public record OpinionChanged
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }

    /// <summary>
    ///     The beer opinions count.
    /// </summary>
    public int OpinionsCount { get; set; }

    /// <summary>
    ///     The new beer rating.
    /// </summary>
    public double NewBeerRating { get; set; }
}