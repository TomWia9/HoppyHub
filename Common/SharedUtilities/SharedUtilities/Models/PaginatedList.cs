using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SharedUtilities.Models;

/// <summary>
///     PaginatedList class.
/// </summary>
/// <typeparam name="T">The type of the list</typeparam>
public class PaginatedList<T> : List<T>
{
    /// <summary>
    ///     Initializes PaginatedList.
    /// </summary>
    /// <param name="items">The items</param>
    /// <param name="count">The items count</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    private PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    /// <summary>
    ///     The current page.
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    ///     The total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    ///     The page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    ///     The total count of items.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    ///     Indicates whether the list has a previous page.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    ///     Indicates whether the list has a next page.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    ///     Converts queryable to the paginated list.
    /// </summary>
    /// <param name="source">The source with specified type</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>Paginated list of specified type</returns>
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    ///     Converts enumerable to the paginated list.
    /// </summary>
    /// <param name="source">The source with specified type</param>
    /// <param name="pageNumber">The page number</param>
    /// <param name="pageSize">The page size</param>
    /// <returns>Paginated list of specified type</returns>
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber,
        int pageSize)
    {
        var collection = source.ToList();
        var count = collection.Count;
        var items = collection.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>
    ///     Gets metadata from paginated list.
    /// </summary>
    /// <returns>Serialized metadata</returns>
    public string GetMetadata()
    {
        var metadata = new
        {
            TotalCount,
            PageSize,
            CurrentPage,
            TotalPages,
            HasNext,
            HasPrevious
        };

        return JsonConvert.SerializeObject(metadata);
    }
}