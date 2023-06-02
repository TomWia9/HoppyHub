namespace Application.Beers.Commands.Common;

/// <summary>
///     BaseBeerCommand abstract record.
/// </summary>
public abstract record BaseBeerCommand
{
    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid BreweryId { get; init; }

    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; init; }

    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    ///     The beer composition.
    /// </summary>
    public string? Composition { get; init; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; init; }

    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid BeerStyleId { get; init; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; init; }
    
    /// <summary>
    ///     The beer release date.
    /// </summary>
    public DateOnly? ReleaseDate { get; init; }
}