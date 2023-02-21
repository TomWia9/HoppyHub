using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The beer
/// </summary>
public class Beer : BaseAuditableEntity
{
    /// <summary>
    ///     The beer name
    /// </summary>
    public string? Name { get; set; }
}