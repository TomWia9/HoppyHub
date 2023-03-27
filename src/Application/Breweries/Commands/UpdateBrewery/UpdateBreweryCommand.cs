using MediatR;

namespace Application.Breweries.Commands.UpdateBrewery;

/// <summary>
///     UpdateBrewery command.
/// </summary>
public record UpdateBreweryCommand : IRequest
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
    ///     The street.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    ///     The house number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    ///     The post code.
    /// </summary>
    public string? PostCode { get; set; }

    /// <summary>
    ///     The city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    ///     The state.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    ///     The country.
    /// </summary>
    public string? Country { get; set; }
}