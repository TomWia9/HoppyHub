using Application.Common.Mappings;
using Domain.Entities;

namespace Application.PrimaryBeerStyles.Dtos;

/// <summary>
///     The primary beer style data transfer object.
/// </summary>
public record PrimaryBeerStyleDto : IMapFrom<PrimaryBeerStyle>
{
    /// <summary>
    ///     The primary beer style name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The primary beer style description.
    /// </summary>
    public string? Description { get; set; }
}