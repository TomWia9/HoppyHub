using Application.Common.Models;
using Application.Identity.Commands.Common;
using MediatR;

namespace Application.Identity.Commands.RegisterUser;

/// <summary>
///     RegisterUser command.
/// </summary>
public record RegisterUserCommand : BaseIdentityCommand, IRequest<AuthenticationResult>
{
    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; init; }
}