using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Users.Queries;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

/// <summary>
///     UsersService class.
/// </summary>
public class UsersService : IUsersService
{
    /// <summary>
    ///     The user manager.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    ///     Initializes UsersService.
    /// </summary>
    /// <param name="userManager">The user manager</param>
    public UsersService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    ///     Gets user by id.
    /// </summary>
    /// <param name="userId">The user id</param>
    public async Task<UserDto> GetUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), userId);
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            Role = userRoles.FirstOrDefault()
        };
    }
}