namespace ImageManagement.Models;

/// <summary>
///     The upload result model.
/// </summary>
public record UploadResult : Result
{
    /// <summary>
    ///     The file uri.
    /// </summary>
    public string? Uri { get; set; }
}