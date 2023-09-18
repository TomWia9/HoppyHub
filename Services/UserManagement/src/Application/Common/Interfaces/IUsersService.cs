using Application.Common.Models;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
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

    /// <summary>
    ///     Gets username by user id.
    /// </summary>
    /// <param name="userId">The user id</param>
    Task<string?> GetUsernameAsync(Guid userId);

    /// <summary>
    ///     Updates user.
    /// </summary>
    /// <param name="request">Update user command</param>
    Task UpdateUserAsync(UpdateUserCommand request);

    /// <summary>
    ///     Deletes user.
    /// </summary>
    /// <param name="request">Delete user command</param>
    Task DeleteUserAsync(DeleteUserCommand request);
}