using Application.Beers.Dtos;
using Application.Common.Abstractions;
using Application.Common.Models;
using MediatR;

namespace Application.Favorites.Queries.GetFavorites;

/// <summary>
///     GetFavorites query.
/// </summary>
public record GetFavoritesQuery : QueryParameters, IRequest<PaginatedList<BeerDto>>
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }
}