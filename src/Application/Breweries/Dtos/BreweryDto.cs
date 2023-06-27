using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Breweries.Dtos;

/// <summary>
///     The brewery data transfer object.
/// </summary>
public record BreweryDto : IMapFrom<Brewery>
{
    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The brewery name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The foundation year.
    /// </summary>
    public int FoundationYear { get; set; }

    /// <summary>
    ///     The website url.
    /// </summary>
    public string? WebsiteUrl { get; set; }

    /// <summary>
    ///     The brewery address.
    /// </summary>
    public AddressDto? Address { get; set; }
}