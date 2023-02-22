using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Identity.Commands.RegisterUser;

/// <summary>
///     RegisterUser handler.
/// </summary>
public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    /// <summary>
    ///     The identity service.
    /// </summary>
    private readonly IIdentityService _identityService;

    public RegisterUserHandler(IIdentityService identityService)
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