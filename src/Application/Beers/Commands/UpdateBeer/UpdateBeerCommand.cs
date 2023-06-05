using Application.Beers.Commands.Common;
using MediatR;

namespace Application.Beers.Commands.UpdateBeer;

/// <summary>
///     UpdateBeer command.
/// </summary>
public record UpdateBeerCommand : BaseBeerCommand, IRequest
{
    /// <summary>
    ///     The beer Id.
    /// </summary>
    public Guid Id { get; init; }
}