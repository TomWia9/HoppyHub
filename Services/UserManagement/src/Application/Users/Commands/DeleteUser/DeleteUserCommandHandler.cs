using Application.Common.Interfaces;
using MediatR;
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
    ///     Initializes DeleteUserCommandHandler.
    /// </summary>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="usersService">The users service</param>
    public DeleteUserCommandHandler(ICurrentUserService currentUserService, IUsersService usersService)
    {
        _currentUserService = currentUserService;
        _usersService = usersService;
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
    }
}