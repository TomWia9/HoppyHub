using Application.BeerStyles.Dtos;
using Application.Breweries.Dtos;
using AutoMapper;
using Domain.Entities;
using SharedUtilities.Mappings;

namespace Application.Beers.Dtos;

/// <summary>
///     The beer data transfer object.
/// </summary>
public record BeerDto : IMapFrom<Beer>
{
    /// <summary>
    ///     The beer Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The brewery.
    /// </summary>
    public BreweryDto? Brewery { get; set; }

    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; set; }

    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The beer composition.
    /// </summary>
    public string? Composition { get; set; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; set; }

    /// <summary>
    ///     The beer average rating.
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    ///     The beer style.
    /// </summary>
    public BeerStyleDto? BeerStyle { get; set; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; set; }

    /// <summary>
    ///     The beer release date.
    /// </summary>
    public DateOnly? ReleaseDate { get; set; }

    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; set; }
    
    /// <summary>
    ///     Indicates whether beer image is temp.
    /// </summary>
    public bool TempImage { get; set; }

    /// <summary>
    ///     The beer opinions count.
    /// </summary>
    public int OpinionsCount { get; set; }

    /// <summary>
    ///     The beer adds to favorites count.
    /// </summary>
    public int FavoritesCount { get; set; }

    /// <summary>
    ///     Creates Beer - BeerDto map.
    /// </summary>
    /// <param name="profile">The profile</param>
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Beer, BeerDto>()
            .ForMember(x => x.ImageUri, opt => opt.MapFrom(x => x.BeerImage!.ImageUri))
            .ForMember(x => x.TempImage, opt => opt.MapFrom(x => x.BeerImage!.TempImage));
    }
}