using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Api.Services;

/// <summary>
///     Current user service class.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    /// <summary>
    ///     Http context accessor.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Initializes CurrentUserService.
    /// </summary>
    /// <param name="httpContextAccessor">Http context accessor</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Current user id.
    /// </summary>
    public Guid? UserId => GetCurrentUserId();

    /// <summary>
    ///     Current user role.
    /// </summary>
    public string? UserRole => GetCurrentUserRole();

    /// <summary>
    ///     Indicates whether current user has admin access.
    /// </summary>
    public bool AdministratorAccess => GetCurrentUserRole() == Roles.Administrator;

    /// <summary>
    ///     Gets current user id.
    /// </summary>
    private Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
    }

    /// <summary>
    ///     Gets current user role.
    /// </summary>
    private string? GetCurrentUserRole()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            if (_httpContextAccessor.HttpContext.User.IsInRole(Roles.User)) return Roles.User;

            if (_httpContextAccessor.HttpContext.User.IsInRole(Roles.Administrator)) return Roles.Administrator;
        }

        return null;
    }
}