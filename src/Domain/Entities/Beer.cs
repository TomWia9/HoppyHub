using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The beer entity class.
/// </summary>
public class Beer : BaseAuditableEntity
{
    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid BeerStyleId { get; set; }

    /// <summary>
    ///     The beer style.
    /// </summary>
    public BeerStyle? BeerStyle { get; set; }

    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; set; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; set; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; set; }

    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The beer composition.
    /// </summary>
    public string? Composition { get; set; }

    /// <summary>
    ///     The beer release date.
    /// </summary>
    public DateOnly? ReleaseDate { get; set; }

    /// <summary>
    ///     The beer average rating.
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid BreweryId { get; set; }

    /// <summary>
    ///     The brewery.
    /// </summary>
    public Brewery? Brewery { get; set; }

    /// <summary>
    ///     The beer opinions.
    /// </summary>
    public ICollection<Opinion> Opinions { get; set; } = new List<Opinion>();

    /// <summary>
    ///     The favorites.
    /// </summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}