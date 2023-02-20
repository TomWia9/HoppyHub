using System.Security.Claims;
using Application.Common.Interfaces;

namespace Api.Services;

/// <summary>
///     Current user service class
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    /// <summary>
    ///     Http context accessor
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Initializes CurrentUserService
    /// </summary>
    /// <param name="httpContextAccessor">Http context accessor</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Current user id
    /// </summary>
    public Guid? UserId => GetCurrentUserId();
    
    /// <summary>
    ///     Gets current user id
    /// </summary>
    /// <returns>Current user id</returns>
    private Guid? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("id");
        return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
    }
}