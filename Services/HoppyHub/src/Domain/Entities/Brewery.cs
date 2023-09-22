using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The brewery entity class.
/// </summary>
public class Brewery : BaseAuditableEntity
{
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
    public Address? Address { get; set; }

    /// <summary>
    ///     The beers.
    /// </summary>
    public ICollection<Beer> Beers { get; set; } = new List<Beer>();
}