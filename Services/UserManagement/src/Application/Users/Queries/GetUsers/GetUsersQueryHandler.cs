using Application.Common.Interfaces;
using Application.Users.Dtos;
using MediatR;
using SharedUtilities.Models;

namespace Application.Users.Queries.GetUsers;

/// <summary>
///     GetUsersQuery handler.
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserDto>>
{
    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     Initializes GetUsersQueryHandler.
    /// </summary>
    /// <param name="usersService">The users service</param>
    public GetUsersQueryHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>
    ///     Handles GetUsersQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _usersService.GetUsersAsync(request);
    }
}