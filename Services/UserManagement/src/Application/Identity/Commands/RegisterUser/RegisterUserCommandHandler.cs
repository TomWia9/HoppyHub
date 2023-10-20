using Application.Common.Interfaces;
using Application.Common.Models;
using MassTransit;
using MediatR;
using SharedEvents;
using SharedUtilities.Models;

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
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;
    
    /// <summary>
    ///     Initializes RegisterUserCommandHandler
    /// </summary>
    /// <param name="identityService">The identity service</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public RegisterUserCommandHandler(IIdentityService identityService, IPublishEndpoint publishEndpoint)
    {
        _identityService = identityService;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles user registration.
    /// </summary>
    /// <param name="request">The register user request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var authenticationResult = await _identityService.RegisterAsync(request.Email, request.Username, request.Password);
        
        if (!authenticationResult.Succeeded || string.IsNullOrEmpty(authenticationResult.Token))
        {
            return authenticationResult;
        }

        var newUserId = _identityService.GetUserIdFromJwt(authenticationResult.Token);

        if (!string.IsNullOrEmpty(newUserId))
        {
            await _publishEndpoint.Publish<UserCreated>(new
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Role = Roles.User
            }, cancellationToken);
        }

        return authenticationResult;
    }
}