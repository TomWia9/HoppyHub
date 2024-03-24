namespace ImageManagement.Models;

/// <summary>
///     The upload response model.
/// </summary>
public class UploadResponse
{
    /// <summary>
    ///     Indicates whether file upload operation succeeded.  
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     The file uri.
    /// </summary>
    public string? Uri { get; set; }

    /// <summary>
    ///     The error message.
    /// </summary>
    public string? ErrorMessage { get; set; }
}