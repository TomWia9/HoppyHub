using MediatR;

namespace Application.Breweries.Commands.DeleteBrewery;

/// <summary>
///     DeleteBrewery command
/// </summary>
public record DeleteBreweryCommand : IRequest
{
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid Id { get; init; }
}