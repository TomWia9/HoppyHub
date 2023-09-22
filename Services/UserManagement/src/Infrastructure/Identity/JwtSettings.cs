namespace Infrastructure.Identity;

/// <summary>
///     JwtSettings class.
/// </summary>
public class JwtSettings
{
    /// <summary>
    ///     The token secret key.
    /// </summary>
    public string? Secret { get; set; }
}