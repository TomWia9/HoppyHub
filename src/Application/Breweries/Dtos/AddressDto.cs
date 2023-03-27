using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Breweries.Dtos;

/// <summary>
///     The address data transfer object.
/// </summary>
public record AddressDto : IMapFrom<Address>
{
    /// <summary>
    ///     The address id.
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
}