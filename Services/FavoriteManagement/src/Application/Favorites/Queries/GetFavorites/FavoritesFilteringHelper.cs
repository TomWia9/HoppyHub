using System.Linq.Expressions;
using Domain.Entities;
using SharedUtilities.Abstractions;

namespace Application.Favorites.Queries.GetFavorites;

/// <summary>
///     FavoritesFilteringHelper class.
/// </summary>
public class FavoritesFilteringHelper : FilteringHelperBase<Favorite, GetFavoritesQuery>
{
    /// <summary>
    ///     Favorites sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Favorite, object>>> SortingColumns = new()
    {
        { nameof(Favorite.LastModified).ToUpper(), x => x.LastModified ?? new DateTimeOffset() },
        { nameof(Favorite.Beer).ToUpper(), x => x.Beer!.Name ?? string.Empty }
    };

    /// <summary>
    ///     Initializes FavoritesFilteringHelper.
    /// </summary>
    public FavoritesFilteringHelper() : base(SortingColumns)
    {
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetFavoritesQuery</param>
    public override IEnumerable<Expression<Func<Favorite, bool>>> GetDelegates(GetFavoritesQuery request)
    {
        var delegates = new List<Expression<Func<Favorite, bool>>>
        {
            x => x.CreatedBy == request.UserId,
        };

        if (request.BeerId is not null)
            delegates.Add(x => x.Beer != null && x.Beer.Id == request.BeerId);

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