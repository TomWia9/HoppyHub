using Application.Common.Interfaces;
using MassTransit;
using MediatR;
using SharedEvents.Events;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.Users.Commands.DeleteUser;

/// <summary>
///     DeleteUserCommand handler.
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes DeleteUserCommandHandler.
    /// </summary>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="usersService">The users service</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteUserCommandHandler(ICurrentUserService currentUserService, IUsersService usersService,
        IPublishEndpoint publishEndpoint)
    {
        _currentUserService = currentUserService;
        _usersService = usersService;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles DeleteUserCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (request.UserId != currentUserId && !_currentUserService.AdministratorAccess)
        {
            throw new ForbiddenAccessException();
        }

        await _usersService.DeleteUserAsync(request);

        var userDeletedEvent = new UserDeleted
        {
            Id = request.UserId
        };

        await _publishEndpoint.Publish(userDeletedEvent, cancellationToken);
    }
}