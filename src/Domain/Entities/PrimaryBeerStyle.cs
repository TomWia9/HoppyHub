using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The primary beer style entity.
/// </summary>
public class PrimaryBeerStyle : BaseAuditableEntity
{
    /// <summary>
    ///     The primary beer style name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The primary beer style description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The secondary beer styles.
    /// </summary>
    public ICollection<SecondaryBeerStyle> SecondaryBeerStyles { get; set; } = new List<SecondaryBeerStyle>();
}