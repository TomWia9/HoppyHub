using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Queries.GetUsers;
using Infrastructure.Helpers;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
    ///     The query service.
    /// </summary>
    private readonly IQueryService<ApplicationUser> _queryService;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     Initializes UsersService.
    /// </summary>
    /// <param name="userManager">The user manager</param>
    /// <param name="queryService">The query service</param>
    /// <param name="currentUserService">The current user service</param>
    public UsersService(UserManager<ApplicationUser> userManager, IQueryService<ApplicationUser> queryService,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _queryService = queryService;
        _currentUserService = currentUserService;
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

        return await MapUser(user);
    }

    /// <summary>
    ///     Gets username by user id.
    /// </summary>
    /// <param name="userId">The user id</param>
    public async Task<string?> GetUsernameAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user?.UserName;
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

        var delegates = UsersFilteringHelper.GetDelegates(request);
        var sortingColumn = UsersFilteringHelper.GetSortingColumn(request.SortBy);

        usersCollection = _queryService.Filter(usersCollection, delegates);
        usersCollection = _queryService.Sort(usersCollection, sortingColumn, request.SortDirection);

        var mappedUsers = new List<UserDto>();

        foreach (var user in usersCollection.ToList())
        {
            mappedUsers.Add(await MapUser(user));
        }

        return mappedUsers.ToPaginatedList(request.PageNumber, request.PageSize);
    }

    /// <summary>
    ///     Gets users dictionary with id as a key and username as a value.
    /// </summary>
    public async Task<Dictionary<Guid, string?>> GetUsersAsync()
    {
        var users = await _userManager.Users
            .Select(x => new { x.Id, x.UserName })
            .ToListAsync();

        var usersDictionary = users.ToDictionary(x => x.Id, x => x.UserName);

        return usersDictionary;
    }

    /// <summary>
    ///     Updates user.
    /// </summary>
    /// <param name="request">Update user command</param>
    public async Task UpdateUserAsync(UpdateUserCommand request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            user.UserName = request.Username;
            var updateUserResult = await _userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                throw new BadRequestException(string.Join(", ", updateUserResult.Errors.Select(x => x.Description)));
            }
        }

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
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);
        }

        if (!_currentUserService.AdministratorAccess && !await _userManager.CheckPasswordAsync(user, request.Password!))
            throw new BadRequestException("Provided password is incorrect");

        await _userManager.DeleteAsync(user);
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
            Role = userRole.FirstOrDefault()
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

            if (newPasswordErrors.Any())
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
}