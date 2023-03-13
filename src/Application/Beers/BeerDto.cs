using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Beers;

/// <summary>
///     The beer data transfer object.
/// </summary>
public record BeerDto : IMapFrom<Beer>
{
    /// <summary>
    ///     The beer Id.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    ///     The brewery.
    /// </summary>
    public string? Brewery { get; set; }
    
    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; set; }
    
    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    ///     The extract in Specific Gravity units.
    /// </summary>
    public double? SpecificGravity { get; set; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; set; }

    /// <summary>
    ///     The extract in Plato units.
    /// </summary>
    public double? Plato { get; set; }

    /// <summary>
    ///     The beer style.
    /// </summary>
    public string? Style { get; set; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; set; }

    /// <summary>
    ///     The country of origin.
    /// </summary>
    public string? Country { get; set; }
}