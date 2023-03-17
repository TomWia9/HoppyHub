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
    ///     The beer style.
    /// </summary>
    public string? Style { get; set; }
    
    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; set; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; set; }

    /// <summary>
    ///     The extract in Plato units.
    /// </summary>
    public double? Plato { get; set; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; set; }

    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid BreweryId { get; set; }
    
    /// <summary>
    ///     The brewery.
    /// </summary>
    public Brewery? Brewery { get; set; }
}