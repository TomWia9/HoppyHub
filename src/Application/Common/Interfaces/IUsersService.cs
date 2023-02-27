using Application.Users.Queries;

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
}