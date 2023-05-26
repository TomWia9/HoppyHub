using Application.Beers.Dtos;

namespace Application.Favorites.Dtos;

/// <summary>
///     The favorites list transfer object.
/// </summary>
public record FavoritesListDto //: IMapFrom<Favorite>
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     The favorites beers.
    /// </summary>
    public IEnumerable<BeerDto> Beers { get; set; }

    // /// <summary>
    // ///     Creates Favorite - FavoriteDto map.
    // /// </summary>
    // /// <param name="profile">The profile</param>
    // public void Mapping(Profile profile)
    // {
    //     profile.CreateMap<Favorite, FavoritesListDto>()
    //         .ForMember(x => x.Username, opt => opt.Ignore());
    // }
}