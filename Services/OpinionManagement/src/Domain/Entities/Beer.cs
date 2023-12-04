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
    ///     The beer brewery id
    /// </summary>
    public Guid BreweryId { get; set; }

    /// <summary>
    ///     The beer brewery name.
    /// </summary>
    public string? BreweryName { get; set; }

    /// <summary>
    ///     The favorites.
    /// </summary>
    public ICollection<Opinion> Opinions { get; set; } = new List<Opinion>();
}