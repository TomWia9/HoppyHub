namespace Application.Common.Interfaces;

/// <summary>
///     AppConfiguration interface.
/// </summary>
public interface IAppConfiguration
{
    /// <summary>
    ///     Temporary beer image uri.
    /// </summary>
    string TempBeerImageUri { get; }

    /// <summary>
    ///     The token secret key.
    /// </summary>
    string JwtSecret { get; }
}