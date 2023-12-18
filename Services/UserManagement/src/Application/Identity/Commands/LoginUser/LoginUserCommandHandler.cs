using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Identity.Commands.LoginUser;

/// <summary>
///     LoginUserCommand handler.
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationResult>
{
    /// <summary>
    ///     The identity service.
    /// </summary>
    private readonly IIdentityService _identityService;

    /// <summary>
    ///     Initializes LoginUserCommandHandler.
    /// </summary>
    /// <param name="identityService">The identity service</param>
    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    /// <summary>
    ///     Handles user login.
    /// </summary>
    /// <param name="request">The login user request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<AuthenticationResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Email, request.Password);
    }
}