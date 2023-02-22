using Application.Common.Models;
using MediatR;

namespace Application.Identity.Commands.LoginUser;

/// <summary>
///     LoginUser command.
/// </summary>
public class LoginUserCommand : IRequest<AuthenticationResult>
{
    /// <summary>
    ///     The user email.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    ///     The password.
    /// </summary>
    public string? Password { get; init; }
}