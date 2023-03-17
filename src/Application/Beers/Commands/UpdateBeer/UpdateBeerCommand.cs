using MediatR;

namespace Application.Beers.Commands.UpdateBeer;

/// <summary>
///     UpdateBeer command.
/// </summary>
public record UpdateBeerCommand : IRequest
{
    /// <summary>
    ///     The beer Id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid BreweryId { get; init; }

    /// <summary>
    ///     The alcohol by volume.
    /// </summary>
    public double AlcoholByVolume { get; init; }

    /// <summary>
    ///     The beer description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The extract in Balling units.
    /// </summary>
    public double? Blg { get; init; }

    /// <summary>
    ///     The extract in Plato units.
    /// </summary>
    public double? Plato { get; init; }

    /// <summary>
    ///     The beer style.
    /// </summary>
    public string? Style { get; init; }

    /// <summary>
    ///     The International Bitterness Units.
    /// </summary>
    public int? Ibu { get; init; }
}