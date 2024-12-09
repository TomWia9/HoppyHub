using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUsername;
using Application.Users.Commands.UpdateUserPassword;
using Application.Users.Dtos;
using Application.Users.Queries.GetUsers;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using SharedUtilities.Enums;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;
using SharedUtilities.Mappings;
using SharedUtilities.Models;

namespace Infrastructure.Services;

/// <summary>
///     UsersService class.
/// </summary>
public class UsersService : IUsersService
{
    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The user manager.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    ///     Initializes UsersService.
    /// </summary>
    /// <param name="userManager">The user manager</param>
    /// <param name="currentUserService">The current user service</param>
    public UsersService(UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Gets user by id.
    /// </summary>
    /// <param name="userId">The user id</param>
    public async Task<UserDto> GetUserAsync(Guid userId)
    {
        var user = await GetUser(userId.ToString());

        return await MapUser(user);
    }

    /// <summary>
    ///     Gets users.
    /// </summary>
    /// <param name="request">Get users query</param>
    public async Task<PaginatedList<UserDto>> GetUsersAsync(GetUsersQuery request)
    {
        var usersCollection = _userManager.Users;

        if (!string.IsNullOrEmpty(request.Role))
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);
            usersCollection = usersInRole.AsQueryable();
        }

        var delegates = GetDelegates(request).ToList();
        var sortingColumn = GetSortingColumn(request.SortBy);

        foreach (var @delegate in delegates)
        {
            usersCollection = usersCollection.Where(@delegate);
        }

        usersCollection = request.SortDirection == SortDirection.Asc
            ? usersCollection.OrderBy(sortingColumn)
            : usersCollection.OrderByDescending(sortingColumn);

        var mappedUsers = new List<UserDto>();

        foreach (var user in usersCollection.ToList())
        {
            mappedUsers.Add(await MapUser(user));
        }

        return mappedUsers.ToPaginatedList(request.PageNumber, request.PageSize);
    }

    /// <summary>
    ///     Updates user.
    /// </summary>
    /// <param name="request">Update user command</param>
    public async Task UpdateUserAsync(UpdateUsernameCommand request)
    {
        var user = await GetUser(request.UserId.ToString());

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            user.UserName = request.Username;
            var updateUserResult = await _userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", updateUserResult.Errors.Select(x => x.Description)));
            }
        }
    }

    /// <summary>
    ///     Changes user password.
    /// </summary>
    /// <param name="request">Update user password command</param>
    public async Task ChangePasswordAsync(UpdateUserPasswordCommand request)
    {
        var user = await GetUser(request.UserId.ToString());

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            await ChangePassword(user, request.CurrentPassword!, request.NewPassword);
        }
    }

    /// <summary>
    ///     Deletes user.
    /// </summary>
    /// <param name="request">Delete user command</param>
    public async Task DeleteUserAsync(DeleteUserCommand request)
    {
        var user = await GetUser(request.UserId.ToString());

        if (!_currentUserService.AdministratorAccess && !await _userManager.CheckPasswordAsync(user, request.Password!))
            throw new BadRequestException("Provided password is incorrect");

        await _userManager.DeleteAsync(user);
    }

    private async Task<ApplicationUser> GetUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(nameof(ApplicationUser), userId);
        }

        return user;
    }

    /// <summary>
    ///     Maps ApplicationUser into UserDto.
    /// </summary>
    /// <param name="user">The ApplicationUser</param>
    private async Task<UserDto> MapUser(ApplicationUser user)
    {
        var userRole = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            Role = userRole.FirstOrDefault(),
            Created = user.Created
        };
    }

    private async Task ChangePassword(ApplicationUser user, string currentPassword, string newPassword)
    {
        if (_currentUserService.AdministratorAccess)
        {
            var newPasswordErrors = new List<string>();
            foreach (var validator in _userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(_userManager, user, newPassword);
                if (!validationResult.Succeeded)
                {
                    newPasswordErrors.Add(string.Join(" ", validationResult.Errors.Select(x => x.Description)));
                }
            }

            if (newPasswordErrors.Count != 0)
            {
                throw new BadRequestException(string.Join(", ", newPasswordErrors.Select(x => x)));
            }

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            if (!removePasswordResult.Succeeded)
                throw new BadRequestException(string.Join(", ",
                    removePasswordResult.Errors.Select(x => x.Description)));

            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

            if (!addPasswordResult.Succeeded)
                throw new BadRequestException(string.Join(", ",
                    addPasswordResult.Errors.Select(x => x.Description)));
        }
        else
        {
            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!changePasswordResult.Succeeded)
                throw new BadRequestException(string.Join(", ",
                    changePasswordResult.Errors.Select(x => x.Description)));
        }
    }

    /// <summary>
    ///     Gets filtering and searching delegates for users.
    /// </summary>
    /// <param name="request">The GetUsersQuery</param>
    private static IEnumerable<Expression<Func<ApplicationUser, bool>>> GetDelegates(GetUsersQuery request)
    {
        var delegates = new List<Expression<Func<ApplicationUser, bool>>>();

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();
        Expression<Func<ApplicationUser, bool>> searchDelegate =
            x => (x.Email != null && x.Email.ToUpper().Contains(searchQuery)) ||
                 (x.UserName != null && x.UserName.ToUpper().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    private static Expression<Func<ApplicationUser, object>> GetSortingColumn(string? sortBy)
    {
        Dictionary<string, Expression<Func<ApplicationUser, object>>> sortingColumns = new()
        {
            { nameof(ApplicationUser.Email).ToUpper(), u => u.Email ?? string.Empty },
            { nameof(ApplicationUser.UserName).ToUpper(), u => u.UserName ?? string.Empty },
            { nameof(ApplicationUser.Created).ToUpper(), u => u.Created }
        };

        return string.IsNullOrEmpty(sortBy) ? sortingColumns.First().Value : sortingColumns[sortBy.ToUpper()];
    }
}