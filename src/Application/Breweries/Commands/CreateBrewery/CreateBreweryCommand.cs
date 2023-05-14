using Application.Breweries.Dtos;
using MediatR;

namespace Application.Breweries.Commands.CreateBrewery;

/// <summary>
///     CreateBrewery command.
/// </summary>
public record CreateBreweryCommand : IRequest<BreweryDto>
{
    /// <summary>
    ///     The brewery name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The foundation year.
    /// </summary>
    public int FoundationYear { get; init; }

    /// <summary>
    ///     The website url.
    /// </summary>
    public string? WebsiteUrl { get; init; }

    /// <summary>
    ///     The street.
    /// </summary>
    public string? Street { get; init; }

    /// <summary>
    ///     The house number.
    /// </summary>
    public string? Number { get; init; }

    /// <summary>
    ///     The post code.
    /// </summary>
    public string? PostCode { get; init; }

    /// <summary>
    ///     The city.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    ///     The state.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    ///     The country.
    /// </summary>
    public string? Country { get; init; }
}