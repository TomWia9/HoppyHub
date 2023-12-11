namespace Domain.Entities;

/// <summary>
///     The user entity class.
/// </summary>
public class User
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     The user role.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    ///     The user favorite beers.
    /// </summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}