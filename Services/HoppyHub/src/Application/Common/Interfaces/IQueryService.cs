using System.Linq.Expressions;
using Application.Common.Enums;

namespace Application.Common.Interfaces;

/// <summary>
///     QueryService interface.
/// </summary>
public interface IQueryService<T> where T : class
{
    /// <summary>
    ///     Filters collection by given predicates.
    /// </summary>
    /// <param name="collection">The Queryable collection</param>
    /// <param name="predicates">The predicates</param>
    IQueryable<T> Filter(IQueryable<T> collection, IEnumerable<Expression<Func<T, bool>>> predicates);

    /// <summary>
    ///     Sorts the given collection on the given property in the specified direction.
    /// </summary>
    /// <param name="collection">The collection to sort</param>
    /// <param name="sortingExpression">The sorting expression</param>
    /// <param name="sortDirection">The sorting direction</param>
    /// <returns>Sorted collection of type IQueryable</returns>
    IQueryable<T> Sort(IQueryable<T> collection, Expression<Func<T, object>> sortingExpression,
        SortDirection sortDirection);
}