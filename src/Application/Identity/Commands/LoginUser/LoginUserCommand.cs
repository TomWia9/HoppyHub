using Application.Common.Models;
using Application.Identity.Commands.Common;
using MediatR;

namespace Application.Identity.Commands.LoginUser;

/// <summary>
///     LoginUser command.
/// </summary>
public record LoginUserCommand : BaseIdentityCommand, IRequest<AuthenticationResult>;