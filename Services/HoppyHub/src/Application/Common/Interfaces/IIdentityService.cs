using Application.Common.Models;

namespace Application.Common.Interfaces;

/// <summary>
///     Identity service interface.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    ///     Registers new user.
    /// </summary>
    /// <param name="email">The email</param>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <returns>An AuthenticationResult</returns>
    Task<AuthenticationResult> RegisterAsync(string? email, string? username, string? password);

    /// <summary>
    ///     Authenticates the user.
    /// </summary>
    /// <param name="email">The email</param>
    /// <param name="password">The password</param>
    /// <returns>An AuthenticationResult</returns>
    Task<AuthenticationResult> LoginAsync(string? email, string? password);
}