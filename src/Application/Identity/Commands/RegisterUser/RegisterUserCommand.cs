using Application.Common.Models;
using MediatR;

namespace Application.Identity.Commands.RegisterUser;

/// <summary>
///     RegisterUser command.
/// </summary>
public record RegisterUserCommand : IRequest<AuthenticationResult>
{
    /// <summary>
    ///     The user email.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     The password.
    /// </summary>
    public string? Password { get; init; }
}