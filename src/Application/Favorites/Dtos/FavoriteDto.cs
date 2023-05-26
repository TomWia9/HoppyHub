using Application.Beers.Dtos;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Favorites.Dtos;

public record FavoriteDto : IMapFrom<Favorite>
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     The beer added to favorites.
    /// </summary>
    public BeerDto? Beer { get; set; }

    /// <summary>
    ///     Creates Favorite - FavoriteDto map.
    /// </summary>
    /// <param name="profile">The profile</param>
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Favorite, FavoriteDto>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.CreatedBy));
    }
}