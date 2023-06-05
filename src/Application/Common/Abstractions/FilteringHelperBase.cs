using System.Linq.Expressions;
using Application.Common.Interfaces;

namespace Application.Common.Abstractions;

public abstract class FilteringHelperBase<T, TRequest> : IFilteringHelper<T, TRequest>
{
    /// <summary>
    ///     The sorting columns.
    /// </summary>
    private Dictionary<string, Expression<Func<T, object>>> SortingColumns { get; }

    /// <summary>
    ///     Initializes FilteringHelperBase.
    /// </summary>
    protected FilteringHelperBase(Dictionary<string, Expression<Func<T, object>>> sortingColumns)
    {
        SortingColumns = sortingColumns;
    }

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    public Expression<Func<T, object>> GetSortingColumn(string? sortBy)
    {
        return string.IsNullOrEmpty(sortBy) ? SortingColumns.First().Value : SortingColumns[sortBy.ToUpper()];
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The request</param>
    public abstract IEnumerable<Expression<Func<T, bool>>> GetDelegates(TRequest request);
}