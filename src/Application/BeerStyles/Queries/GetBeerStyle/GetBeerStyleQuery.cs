using Application.BeerStyles.Dtos;
using MediatR;

namespace Application.BeerStyles.Queries.GetBeerStyle;

/// <summary>
///     GetBeerStyle query.
/// </summary>
public record GetBeerStyleQuery : IRequest<BeerStyleDto>
{
    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid Id { get; set; }
}