namespace Application.Models.BlobContainer;

/// <summary>
///     The blob data transfer object.
/// </summary>
public class BlobDto
{
    /// <summary>
    ///     The blob Uri.
    /// </summary>
    public string? Uri { get; set; }

    /// <summary>
    ///     The blob name.
    /// </summary>
    public string? Name { get; set; }
}