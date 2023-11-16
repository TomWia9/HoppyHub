using Application.BeerStyles.Dtos;
using MediatR;
using SharedUtilities.Abstractions;
using SharedUtilities.Models;

namespace Application.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     GetBeerStyles query.
/// </summary>
public record GetBeerStylesQuery : QueryParameters, IRequest<PaginatedList<BeerStyleDto>>
{
    /// <summary>
    ///     The country of beer style origin.
    /// </summary>
    public string? CountryOfOrigin { get; init; }
}