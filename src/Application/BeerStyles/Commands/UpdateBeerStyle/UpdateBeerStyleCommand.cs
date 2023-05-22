using MediatR;

namespace Application.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     UpdateBeerStyle command.
/// </summary>
public record UpdateBeerStyleCommand : IRequest
{
    /// <summary>
    ///     The beer style id.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    ///     The beer style name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The beer style description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The country of beer style origin.
    /// </summary>
    public string? CountryOfOrigin { get; init; }
}