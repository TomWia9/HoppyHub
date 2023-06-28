using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Mappings;

/// <summary>
///     MappingExtensions class.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    ///     Creates paginated list asynchronously.
    /// </summary>
    /// <param name="queryable">The queryable</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    public static Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, int pageNumber, int pageSize) where TDestination : class
    {
        return PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
    }

    /// <summary>
    ///     Creates paginated list.
    /// </summary>
    /// <param name="source">The source</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    public static PaginatedList<TDestination> ToPaginatedList<TDestination>(
        this IEnumerable<TDestination> source, int pageNumber, int pageSize) where TDestination : class
    {
        return PaginatedList<TDestination>.Create(source, pageNumber, pageSize);
    }

    /// <summary>
    ///     Projects queryable to list asynchronously.
    /// </summary>
    /// <param name="queryable">The queryable</param>
    /// <param name="configuration">The mapper configuration</param>
    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable,
        IConfigurationProvider configuration) where TDestination : class
    {
        return queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();
    }
}