﻿using System.Linq.Expressions;
using SharedUtilities.Interfaces;

namespace SharedUtilities.Abstractions;

/// <summary>
///     The filtering helper base class.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TRequest"></typeparam>
public abstract class FilteringHelperBase<T, TRequest> : IFilteringHelper<T, TRequest>
{
    /// <summary>
    ///     Initializes FilteringHelperBase.
    /// </summary>
    protected FilteringHelperBase(Dictionary<string, Expression<Func<T, object>>> sortingColumns)
    {
        SortingColumns = sortingColumns;
    }

    /// <summary>
    ///     The sorting columns.
    /// </summary>
    private Dictionary<string, Expression<Func<T, object>>> SortingColumns { get; }

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