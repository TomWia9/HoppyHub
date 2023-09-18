using MediatR;

namespace Application.Beers.Commands.DeleteBeer;

/// <summary>
///     DeleteBeer command.
/// </summary>
public record DeleteBeerCommand : IRequest
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid Id { get; init; }
}