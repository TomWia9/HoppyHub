using Application.Beers.Dtos;
using Application.Common.Models;

namespace Application.Favorites.Dtos;

/// <summary>
///     The favorites list transfer object.
/// </summary>
public record FavoritesListDto
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     The favorites beers.
    /// </summary>
    public PaginatedList<BeerDto> Beers { get; set; }
}