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

    /// <summary>
    ///     Indicates whether user is in role.
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="role">The user role</param>
    public Task<bool> IsInRoleAsync(Guid userId, string role);

    /// <summary>
    ///     Indicates whether user is in policy.
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <param name="policy">The user policy</param>
    public Task<bool> IsInPolicyAsync(Guid userId, string policy);

}