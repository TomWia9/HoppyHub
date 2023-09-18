using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

/// <summary>
///     UpdateUserCommand handler.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
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
    ///     Initializes UpdateUserCommandHandler.
    /// </summary>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="usersService">The users service</param>
    public UpdateUserCommandHandler(ICurrentUserService currentUserService, IUsersService usersService)
    {
        _currentUserService = currentUserService;
        _usersService = usersService;
    }

    /// <summary>
    ///     Handles UpdateUserCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (request.UserId != currentUserId && !_currentUserService.AdministratorAccess)
            throw new ForbiddenAccessException();

        await _usersService.UpdateUserAsync(request);
    }
}