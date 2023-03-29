using Application.Breweries.Dtos;
using MediatR;

namespace Application.Breweries.Queries.GetBrewery;

/// <summary>
///     GetBrewery query.
/// </summary>
public record GetBreweryQuery : IRequest<BreweryDto>
{
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid Id { get; set; }
}