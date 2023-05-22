using Application.BeerStyles.Dtos;
using MediatR;

namespace Application.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     CreateBeerStyle command.
/// </summary>
public record CreateBeerStyleCommand : IRequest<BeerStyleDto>
{
    /// <summary>
    ///     The beer style name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The beer style description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The country of beer style origin.
    /// </summary>
    public string? CountryOfOrigin { get; init; }
}