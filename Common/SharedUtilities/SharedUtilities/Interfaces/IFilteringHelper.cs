using System.Linq.Expressions;

namespace SharedUtilities.Interfaces;

/// <summary>
///     The filtering helper interface.
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
/// <typeparam name="TRequest">The query type</typeparam>
public interface IFilteringHelper<T, in TRequest>
{
    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    Expression<Func<T, object>> GetSortingColumn(string? sortBy);

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The request</param>
    IEnumerable<Expression<Func<T, bool>>> GetDelegates(TRequest request);
}