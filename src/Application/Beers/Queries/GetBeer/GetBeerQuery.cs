using MediatR;

namespace Application.Beers.Queries.GetBeer;

/// <summary>
///     GetBeer query.
/// </summary>
public class GetBeerQuery : IRequest<BeerDto>
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid Id { get; set; }
}