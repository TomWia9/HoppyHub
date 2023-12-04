using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The user entity class.
/// </summary>
public class User : BaseAuditableEntity
{
    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     The user role.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    ///     The user beer opinions.
    /// </summary>
    public ICollection<Opinion> Opinions { get; set; } = new List<Opinion>();
}