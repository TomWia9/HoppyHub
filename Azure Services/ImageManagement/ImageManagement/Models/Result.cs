namespace ImageManagement.Models;

/// <summary>
///     The result model.
/// </summary>
public abstract record Result
{
    /// <summary>
    ///     Indicates whether file upload operation succeeded.  
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     The error message.
    /// </summary>
    public string? ErrorMessage { get; set; }
}