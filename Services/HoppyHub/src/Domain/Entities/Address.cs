namespace Domain.Entities;

/// <summary>
///     The address entity class.
/// </summary>
public class Address
{
    /// <summary>
    ///     The Id.
    /// </summary>
    public Guid Id { get; set; }

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

    /// <summary>
    ///     The brewery id.
    /// </summary>
    public Guid BreweryId { get; set; }

    /// <summary>
    ///     The brewery.
    /// </summary>
    public Brewery? Brewery { get; set; }
}