using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUsername;
using Application.Users.Commands.UpdateUserPassword;
using Application.Users.Dtos;
using Application.Users.Queries.GetUsers;
using SharedUtilities.Models;

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
    ///     Updates user.
    /// </summary>
    /// <param name="request">Update user command</param>
    Task UpdateUserAsync(UpdateUsernameCommand request);

    /// <summary>
    ///     Changes user password.
    /// </summary>
    /// <param name="request">Update user password command</param>
    Task ChangePasswordAsync(UpdateUserPasswordCommand request);

    /// <summary>
    ///     Deletes user.
    /// </summary>
    /// <param name="request">Delete user command</param>
    Task DeleteUserAsync(DeleteUserCommand request);
}