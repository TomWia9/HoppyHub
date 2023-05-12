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
    ///     The beer styles.
    /// </summary>
    public ICollection<BeerStyle> BeerStyles { get; set; } = new List<BeerStyle>();
}