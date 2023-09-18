using Application.Common.Models;

namespace Application.Common.Mappings;

/// <summary>
///     MappingExtensions class.
/// </summary>
public static class MappingExtensions
{
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
}