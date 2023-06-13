namespace Domain.Entities;

/// <summary>
///     The beer image entity class.
/// </summary>
public class BeerImage
{
    /// <summary>
    ///     The Id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; set; }

    /// <summary>
    ///     Indicates whether image is temporary.
    /// </summary>
    public bool TempImage { get; set; } = true;
    
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }
    
    /// <summary>
    ///     The beer.
    /// </summary>
    public Beer? Beer { get; set; }
}