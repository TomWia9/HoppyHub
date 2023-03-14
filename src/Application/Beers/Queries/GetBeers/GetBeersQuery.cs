using Application.Common.Abstractions;
using Application.Common.Models;
using MediatR;

namespace Application.Beers.Queries.GetBeers;

/// <summary>
///     GetBeers query.
/// </summary>
public record GetBeersQuery : QueryParameters, IRequest<PaginatedList<BeerDto>>
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
    ///     The beer style.
    /// </summary>
    public string? Style { get; init; }

    /// <summary>
    ///     The country of origin.
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    ///     Minimum alcohol by volume.
    /// </summary>
    public double? MinAlcoholByVolume { get; init; } = 0;

    /// <summary>
    ///     Maximum alcohol by volume.
    /// </summary>
    public double? MaxAlcoholByVolume { get; init; } = 100;

    /// <summary>
    ///     Minimum specific gravity units.
    /// </summary>
    public double? MinSpecificGravity { get; init; } = 1;

    /// <summary>
    ///     Maximum specific gravity units.
    /// </summary>
    public double? MaxSpecificGravity { get; init; } = 1.2;

    /// <summary>
    ///     Minimum Balling units.
    /// </summary>
    public double? MinBlg { get; init; } = 0;

    /// <summary>
    ///     Maximum Balling units.
    /// </summary>
    public double? MaxBlg { get; init; } = 100;

    /// <summary>
    ///     Minimum Plato units.
    /// </summary>
    public double? MinPlato { get; init; } = 0;

    /// <summary>
    ///     Maximum Plato units.
    /// </summary>
    public double? MaxPlato { get; init; } = 100;

    /// <summary>
    ///     Minimum Ibu.
    /// </summary>
    public int? MinIbu { get; init; } = 0;
    
    /// <summary>
    ///     Maximum Ibu.
    /// </summary>
    public int? MaxIbu { get; init; } = 200;
}