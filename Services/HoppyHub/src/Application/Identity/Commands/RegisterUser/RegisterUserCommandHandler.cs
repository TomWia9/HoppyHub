using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Identity.Commands.RegisterUser;

/// <summary>
///     RegisterUserCommand handler.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    /// <summary>
    ///     The identity service.
    /// </summary>
    private readonly IIdentityService _identityService;

    /// <summary>
    ///     Initializes RegisterUserCommandHandler
    /// </summary>
    /// <param name="identityService">The identity service</param>
    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    /// <summary>
    ///     Handles user registration.
    /// </summary>
    /// <param name="request">The register user request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(request.Email, request.Username, request.Password);
    }
}