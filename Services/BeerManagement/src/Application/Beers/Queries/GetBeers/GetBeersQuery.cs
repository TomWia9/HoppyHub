using Application.Beers.Dtos;
using MediatR;
using SharedUtilities.Abstractions;
using SharedUtilities.Models;

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
    ///     The brewery id.
    /// </summary>
    public Guid? BreweryId { get; init; }

    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid? BeerStyleId { get; init; }

    /// <summary>
    ///     Minimum alcohol by volume.
    /// </summary>
    public double? MinAlcoholByVolume { get; init; } = 0;

    /// <summary>
    ///     Maximum alcohol by volume.
    /// </summary>
    public double? MaxAlcoholByVolume { get; init; } = 100;

    /// <summary>
    ///     Minimum blg extract.
    /// </summary>
    public double? MinExtract { get; init; } = 0;

    /// <summary>
    ///     Maximum blg extract.
    /// </summary>
    public double? MaxExtract { get; init; } = 100;

    /// <summary>
    ///     Minimum Ibu.
    /// </summary>
    public int? MinIbu { get; init; } = 0;

    /// <summary>
    ///     Maximum Ibu.
    /// </summary>
    public int? MaxIbu { get; init; } = 200;

    /// <summary>
    ///     Minimum beer release date.
    /// </summary>
    public DateOnly MinReleaseDate { get; init; } = DateOnly.MinValue;

    /// <summary>
    ///     Maximum beer release date.
    /// </summary>
    public DateOnly MaxReleaseDate { get; init; } = DateOnly.FromDateTime(DateTime.Now);

    /// <summary>
    ///     Minimum rating.
    /// </summary>
    public double? MinRating { get; init; } = 0;

    /// <summary>
    ///     Maximum rating.
    /// </summary>
    public double? MaxRating { get; init; } = 10;

    /// <summary>
    ///     Minimum favorites count.
    /// </summary>
    public int? MinFavoritesCount { get; init; } = 0;

    /// <summary>
    ///     Maximum favorites count.
    /// </summary>
    public int? MaxFavoritesCount { get; init; } = int.MaxValue;

    /// <summary>
    ///     Minimum opinion count.
    /// </summary>
    public int? MinOpinionsCount { get; init; } = 0;

    /// <summary>
    ///     Maximum opinion count.
    /// </summary>
    public int? MaxOpinionsCount { get; init; } = int.MaxValue;
}