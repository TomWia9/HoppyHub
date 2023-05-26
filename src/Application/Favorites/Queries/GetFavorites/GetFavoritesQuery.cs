using Application.Common.Abstractions;
using Application.Favorites.Dtos;
using MediatR;

namespace Application.Favorites.Queries.GetFavorites;

/// <summary>
///     GetFavorites query.
/// </summary>
public record GetFavoritesQuery : QueryParameters, IRequest<FavoritesListDto>
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; set; }
}