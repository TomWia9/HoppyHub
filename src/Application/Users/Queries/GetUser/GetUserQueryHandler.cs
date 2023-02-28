using Application.Common.Interfaces;
using MediatR;

namespace Application.Users.Queries.GetUser;

/// <summary>
///     GetUser handler
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     Initializes GetUserHandler
    /// </summary>
    /// <param name="usersService">The users service</param>
    public GetUserQueryHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    ///     Handles GetUserQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _usersService.GetUserAsync(request.UserId);
    }
}