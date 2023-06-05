using Application.BeerStyles.Commands.Common;
using MediatR;

namespace Application.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     UpdateBeerStyle command.
/// </summary>
public record UpdateBeerStyleCommand : BaseBeerStyleCommand, IRequest
{
    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid Id { get; init; }
}