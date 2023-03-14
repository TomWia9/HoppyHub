using MediatR;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeer command.
/// </summary>
public record CreateBeerCommand : IRequest<BeerDto>
{
    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    ///     The brewery.
    /// </summary>
    public string? Brewery { get; init; }
    
    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; init; }
    
    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    ///     The extract in Specific Gravity units.
    /// </summary>
    public double? SpecificGravity { get; init; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; init; }

    /// <summary>
    ///     The extract in Plato units.
    /// </summary>
    public double? Plato { get; init; }

    /// <summary>
    ///     The beer style.
    /// </summary>
    public string? Style { get; init; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; init; }

    /// <summary>
    ///     The country of origin.
    /// </summary>
    public string? Country { get; init; }
}