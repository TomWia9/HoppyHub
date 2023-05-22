using Application.Beers.Dtos;
using MediatR;

namespace Application.Beers.Queries.GetBeer;

/// <summary>
///     GetBeer query.
/// </summary>
public record GetBeerQuery : IRequest<BeerDto>
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid Id { get; set; }
}