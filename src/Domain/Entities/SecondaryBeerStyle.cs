using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The secondary beer style entity.
/// </summary>
public class SecondaryBeerStyle : BaseAuditableEntity
{
    /// <summary>
    ///     The secondary beer style name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The secondary beer style description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The country of style origin.
    /// </summary>
    public string? CountryOfOrigin { get; set; }
    
    /// <summary>
    ///     The primary beer style id.
    /// </summary>
    public Guid PrimaryBeerStyleId { get; set; }
    
    /// <summary>
    ///     The primary beer style.
    /// </summary>
    public PrimaryBeerStyle? PrimaryBeerStyle { get; set; }
}