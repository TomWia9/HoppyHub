using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The favorite entity class.
/// </summary>
public class Favorite : BaseAuditableEntity
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }
    
    /// <summary>
    ///     The beer.
    /// </summary>
    public Beer? Beer { get; set; }
}