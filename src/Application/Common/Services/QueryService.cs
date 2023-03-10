using System.Linq.Expressions;
using Application.Common.Enums;
using Application.Common.Interfaces;

namespace Application.Common.Services;

public class QueryService<T> : IQueryService<T> where T : class
{
    /// <summary>
    ///     Filters collection by given delegates.
    /// </summary>
    /// <param name="collection">The Queryable collection</param>
    /// <param name="delegates">The delegates</param>
    public IQueryable<T> Filter(IQueryable<T> collection, IEnumerable<Expression<Func<T, bool>>> delegates)
    {
        foreach (var @delegate in delegates)
        {
            if (@delegate != null)
            {
                collection = collection.Where(@delegate);
            }
        }

        return collection;
    }

    /// <summary>
    ///     Sorts the given collection on the given property in the specified direction.
    /// </summary>
    /// <param name="collection">The collection to sort</param>
    /// <param name="sortingDelegate">The sorting delegate</param>
    /// <param name="sortDirection">The sorting direction</param>
    /// <returns>Sorted collection of type IQueryable</returns>
    public IQueryable<T> Sort(IQueryable<T> collection, Expression<Func<T, object>> sortingDelegate, SortDirection sortDirection)
    {
        collection = sortDirection == SortDirection.Asc
            ? collection.OrderBy(sortingDelegate)
            : collection.OrderByDescending(sortingDelegate);

        return collection;
    }
}