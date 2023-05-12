using Application.Common.Mappings;
using Application.PrimaryBeerStyles.Dtos;
using Domain.Entities;

namespace Application.BeerStyles.Dtos;

/// <summary>
///     The beer style data transfer object.
/// </summary>
public record BeerStyleDto : IMapFrom<BeerStyle>
{
    /// <summary>
    ///     The beer style name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The beer style description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The country of beer style origin.
    /// </summary>
    public string? CountryOfOrigin { get; set; }
    
    /// <summary>
    ///     The primary beer style.
    /// </summary>
    public PrimaryBeerStyleDto? PrimaryBeerStyle { get; set; }
}