using Domain.Entities;
using SharedUtilities.Mappings;

namespace Application.Favorites.Dtos;

/// <summary>
///     The beer data transfer object.
/// </summary>
public record BeerDto : IMapFrom<Beer>
{
    /// <summary>
    ///     The beer Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The brewery name.
    /// </summary>
    public string? BreweryName { get; set; }
}