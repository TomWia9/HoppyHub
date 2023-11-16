using Application.Breweries.Commands.Common;
using MediatR;

namespace Application.Breweries.Commands.UpdateBrewery;

/// <summary>
///     UpdateBrewery command.
/// </summary>
public record UpdateBreweryCommand : BaseBreweryCommand, IRequest
{
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid Id { get; init; }
}