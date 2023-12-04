namespace Domain.Entities;

/// <summary>
///     The beer entity class.
/// </summary>
public class Beer
{
    /// <summary>
    ///     The entity id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The beer name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The beer brewery name.
    /// </summary>
    public string? BreweryName { get; set; }

    /// <summary>
    ///     The favorites.
    /// </summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}