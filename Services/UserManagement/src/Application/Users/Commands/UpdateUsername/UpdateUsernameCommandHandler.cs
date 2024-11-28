using Application.Common.Interfaces;
using MassTransit;
using MediatR;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.Users.Commands.UpdateUsername;

/// <summary>
///     UpdateUsernameCommand handler.
/// </summary>
public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand>
{
    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     Initializes UpdateUsernameCommandHandler.
    /// </summary>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="usersService">The users service</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public UpdateUsernameCommandHandler(ICurrentUserService currentUserService, IUsersService usersService,
        IPublishEndpoint publishEndpoint)
    {
        _currentUserService = currentUserService;
        _usersService = usersService;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles UpdateUsernameCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (request.UserId != currentUserId && !_currentUserService.AdministratorAccess)
            throw new ForbiddenAccessException();

        await _usersService.UpdateUserAsync(request);

        var userUpdatedEvent = new UserUpdated
        {
            Id = request.UserId,
            Username = request.Username
        };

        await _publishEndpoint.Publish(userUpdatedEvent, cancellationToken);
    }
}