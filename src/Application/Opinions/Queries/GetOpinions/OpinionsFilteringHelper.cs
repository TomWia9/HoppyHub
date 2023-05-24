using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     OpinionsFilteringHelper class.
/// </summary>
public static class OpinionsFilteringHelper
{
    /// <summary>
    ///     Opinions sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Opinion, object>>> SortingColumns = new()
    {
        { nameof(Opinion.LastModified).ToUpper(), x => x.LastModified ?? new DateTime() },
        { nameof(Opinion.Rate).ToUpper(), x => x.Rate },
        { nameof(Opinion.Comment).ToUpper(), x => x.Comment ?? string.Empty },
    };

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    public static Expression<Func<Opinion, object>> GetSortingColumn(string? sortBy)
    {
        return string.IsNullOrEmpty(sortBy) ? SortingColumns.First().Value : SortingColumns[sortBy.ToUpper()];
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetOpinionsQuery</param>
    public static IEnumerable<Expression<Func<Opinion, bool>>> GetDelegates(GetOpinionsQuery request)
    {
        var delegates = new List<Expression<Func<Opinion, bool>>>
        {
            x => x.Rate >= request.MinRate && x.Rate <= request.MaxRate,
        };

        if (request.BeerId != null)
            delegates.Add(x => x.BeerId == request.BeerId);
        
        if (request.UserId != null)
            delegates.Add(x => x.CreatedBy == request.UserId);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Opinion, bool>> searchDelegate =
            x => x.Comment != null && x.Comment.ToUpper().Contains(searchQuery);

        delegates.Add(searchDelegate);

        return delegates;
    }
}