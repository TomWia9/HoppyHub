using Application.Favorites.Dtos;
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
    ///     The beer id.
    /// </summary>
    public Guid? BeerId { get; init; }
    
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }
}