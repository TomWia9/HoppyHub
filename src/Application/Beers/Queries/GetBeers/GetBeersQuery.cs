using Application.Beers.Dtos;
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
    ///     Minimum alcohol by volume.
    /// </summary>
    public double? MinAlcoholByVolume { get; init; } = 0;

    /// <summary>
    ///     Maximum alcohol by volume.
    /// </summary>
    public double? MaxAlcoholByVolume { get; init; } = 100;

    /// <summary>
    ///     Minimum Ibu.
    /// </summary>
    public int? MinIbu { get; init; } = 0;

    /// <summary>
    ///     Maximum Ibu.
    /// </summary>
    public int? MaxIbu { get; init; } = 200;
}