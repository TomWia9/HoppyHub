using Application.Common.Interfaces;
using MediatR;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.Users.Commands.UpdateUserPassword;

/// <summary>
///     UpdateUserPasswordCommand handler.
/// </summary>
public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand>
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
    ///     Initializes UpdateUserPasswordCommandHandler.
    /// </summary>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="usersService">The users service</param>
    public UpdateUserPasswordCommandHandler(ICurrentUserService currentUserService, IUsersService usersService)
    {
        _currentUserService = currentUserService;
        _usersService = usersService;
    }

    /// <summary>
    ///     Handles UpdateUserPasswordCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (request.UserId != currentUserId && !_currentUserService.AdministratorAccess)
            throw new ForbiddenAccessException();

        await _usersService.ChangePasswordAsync(request);
    }
}