using System.Linq.Expressions;
using Application.Users.Queries.GetUsers;
using Infrastructure.Identity;

namespace Infrastructure.Helpers;

/// <summary>
///     UsersFilteringHelper class.
/// </summary>
public static class UsersFilteringHelper
{
    /// <summary>
    ///     Users sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<ApplicationUser, object>>> SortingColumns = new()
    {
        { nameof(ApplicationUser.Email).ToLower(), u => u.Email ?? string.Empty },
        { nameof(ApplicationUser.UserName).ToLower(), u => u.UserName ?? string.Empty }
    };

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    public static Expression<Func<ApplicationUser, object>> GetSortingColumn(string? sortBy)
    {
        return string.IsNullOrEmpty(sortBy) ? SortingColumns.First().Value : SortingColumns[sortBy.ToLower()];
    }

    /// <summary>
    ///     Gets filtering and searching delegates for users
    /// </summary>
    /// <param name="request">The GetUsersQuery</param>
    public static IEnumerable<Expression<Func<ApplicationUser, bool>>> GetDelegates(GetUsersQuery request)
    {
        var delegates = new List<Expression<Func<ApplicationUser, bool>>>();

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToLower();
        Expression<Func<ApplicationUser, bool>> searchDelegate =
            x => x.UserName != null && x.Email != null && (x.Email.ToLower().Contains(searchQuery) ||
                                                           x.UserName.ToLower().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}