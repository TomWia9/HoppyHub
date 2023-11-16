using MediatR;

namespace Application.BeerStyles.Commands.DeleteBeerStyle;

/// <summary>
///     DeleteBeerStyle command.
/// </summary>
public record DeleteBeerStyleCommand : IRequest
{
    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid Id { get; init; }
}