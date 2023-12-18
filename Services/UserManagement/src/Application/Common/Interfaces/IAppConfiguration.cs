namespace Application.Common.Interfaces;

/// <summary>
///     AppConfiguration interface.
/// </summary>
public interface IAppConfiguration
{
    /// <summary>
    ///     The token secret key.
    /// </summary>
    string JwtSecret { get; }
}