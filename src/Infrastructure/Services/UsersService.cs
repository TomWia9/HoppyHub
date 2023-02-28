using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Users.Queries;
using Application.Users.Queries.GetUsers;
using Infrastructure.Helpers;
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
    ///     The query service.
    /// </summary>
    private readonly IQueryService<ApplicationUser> _queryService;

    /// <summary>
    ///     Initializes UsersService.
    /// </summary>
    /// <param name="userManager">The user manager</param>
    /// <param name="queryService">The query service</param>
    public UsersService(UserManager<ApplicationUser> userManager, IQueryService<ApplicationUser> queryService)
    {
        _userManager = userManager;
        _queryService = queryService;
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
}