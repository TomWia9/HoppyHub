using System.Linq.Expressions;
using Application.Common.Enums;
using Application.Common.Interfaces;

namespace Application.Common.Services;

public class QueryService<T> : IQueryService<T> where T : class
{
    /// <summary>
    ///     Filters collection by given predicates.
    /// </summary>
    /// <param name="collection">The Queryable collection</param>
    /// <param name="predicates">The predicates</param>
    public IQueryable<T> Filter(IQueryable<T> collection, IEnumerable<Expression<Func<T, bool>>> predicates)
    {
        foreach (var predicate in predicates)
        {
            collection = collection.Where(predicate);
        }

        return collection;
    }

    /// <summary>
    ///     Sorts the given collection on the given property in the specified direction.
    /// </summary>
    /// <param name="collection">The collection to sort</param>
    /// <param name="sortingExpression">The sorting expression</param>
    /// <param name="sortDirection">The sorting direction</param>
    /// <returns>Sorted collection of type IQueryable</returns>
    public IQueryable<T> Sort(IQueryable<T> collection, Expression<Func<T, object>> sortingExpression,
        SortDirection sortDirection)
    {
        collection = sortDirection == SortDirection.Asc
            ? collection.OrderBy(sortingExpression)
            : collection.OrderByDescending(sortingExpression);

        return collection;
    }
}