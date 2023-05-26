using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Favorites.Queries.GetFavorites;

/// <summary>
///     FavoritesFilteringHelper class.
/// </summary>
public class FavoritesFilteringHelper
{
    /// <summary>
    ///     Favorites sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Favorite, object>>> SortingColumns = new()
    {
        { nameof(Favorite.LastModified).ToUpper(), x => x.LastModified ?? new DateTime() },
        { nameof(Favorite.Beer).ToUpper(), x => x.Beer!.Name ?? string.Empty },
    };

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    public static Expression<Func<Favorite, object>> GetSortingColumn(string? sortBy)
    {
        return string.IsNullOrEmpty(sortBy) ? SortingColumns.First().Value : SortingColumns[sortBy.ToUpper()];
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetFavoritesQuery</param>
    public static IEnumerable<Expression<Func<Favorite, bool>>> GetDelegates(GetFavoritesQuery request)
    {
        var delegates = new List<Expression<Func<Favorite, bool>>>
        {
            x => x.CreatedBy == request.UserId
        };

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Favorite, bool>> searchDelegate =
            x => x.Beer != null && x.Beer.Name != null && x.Beer.Name.ToUpper().Contains(searchQuery);

        delegates.Add(searchDelegate);

        return delegates;
    }
}