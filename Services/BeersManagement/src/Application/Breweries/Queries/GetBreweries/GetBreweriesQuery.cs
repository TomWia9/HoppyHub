using Application.Breweries.Dtos;
using MediatR;
using SharedUtilities.Abstractions;
using SharedUtilities.Models;

namespace Application.Breweries.Queries.GetBreweries;

/// <summary>
///     Get breweries query.
/// </summary>
public record GetBreweriesQuery : QueryParameters, IRequest<PaginatedList<BreweryDto>>
{
    /// <summary>
    ///     The brewery name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The brewery country.
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    ///     The brewery state.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    ///     The brewery city.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    ///     Minimum foundation year.
    /// </summary>
    public double? MinFoundationYear { get; init; } = 0;

    /// <summary>
    ///     Maximum foundation year.
    /// </summary>
    public double? MaxFoundationYear { get; init; } = DateTime.Now.Year;
}