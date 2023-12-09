namespace SharedEvents.Events;

/// <summary>
///     The beer favorites count changed event.
/// </summary>
public record BeerFavoritesCountChanged
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }

    /// <summary>
    ///     The beer favorites count.
    /// </summary>
    public int FavoritesCount { get; set; }
}