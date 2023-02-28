using Application.Common.Models;
using Application.Users.Queries;
using Application.Users.Queries.GetUsers;

namespace Application.Common.Interfaces;

/// <summary>
///     UsersService interface.
/// </summary>
public interface IUsersService
{
    /// <summary>
    ///     Gets user by id.
    /// </summary>
    /// <param name="userId">The user id</param>
    public Task<UserDto> GetUserAsync(Guid userId);
    
    /// <summary>
    ///     Gets users.
    /// </summary>
    /// <param name="request">Get users query</param>
    Task<PaginatedList<UserDto>> GetUsersAsync(GetUsersQuery request);
}