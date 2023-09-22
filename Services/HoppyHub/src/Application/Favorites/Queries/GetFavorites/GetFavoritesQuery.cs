using Application.Beers.Dtos;
using MediatR;
using SharedUtilities.Abstractions;
using SharedUtilities.Models;

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