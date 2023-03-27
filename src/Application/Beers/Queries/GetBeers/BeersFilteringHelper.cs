using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Beers.Queries.GetBeers;

/// <summary>
///     BeersFilteringHelper class.
/// </summary>
public static class BeersFilteringHelper
{
    /// <summary>
    ///     Beers sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Beer, object>>> SortingColumns = new()
    {
        { nameof(Beer.Name).ToUpper(), x => x.Name ?? string.Empty },
        { nameof(Beer.Brewery).ToUpper(), x => x.Brewery!.Name ?? string.Empty },
        { nameof(Beer.Style).ToUpper(), x => x.Style ?? string.Empty },
        { nameof(Beer.AlcoholByVolume).ToUpper(), x => x.AlcoholByVolume },
        { nameof(Beer.Blg).ToUpper(), x => x.Blg ?? 0 },
        { nameof(Beer.Plato).ToUpper(), x => x.Plato ?? 0 },
        { nameof(Beer.Ibu).ToUpper(), x => x.Ibu ?? 0 }
    };

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    public static Expression<Func<Beer, object>> GetSortingColumn(string? sortBy)
    {
        return string.IsNullOrEmpty(sortBy) ? SortingColumns.First().Value : SortingColumns[sortBy.ToUpper()];
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetBeersQuery</param>
    public static IEnumerable<Expression<Func<Beer, bool>>> GetDelegates(GetBeersQuery request)
    {
        var delegates = new List<Expression<Func<Beer, bool>>>
        {
            x => x.AlcoholByVolume >= request.MinAlcoholByVolume && x.AlcoholByVolume <= request.MaxAlcoholByVolume,
            x => x.Ibu >= request.MinIbu && x.Ibu <= request.MaxIbu || x.Ibu == null
        };

        if (!string.IsNullOrWhiteSpace(request.Name))
            delegates.Add(x => x.Name != null && string.Equals(x.Name.ToUpper(), request.Name.ToUpper()));

        if (!string.IsNullOrWhiteSpace(request.Style))
            delegates.Add(x => x.Style != null && string.Equals(x.Style.ToUpper(), request.Style.ToUpper()));

        if (request.BreweryId != null)
            delegates.Add(x => x.Brewery != null && x.Brewery.Id == request.BreweryId);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Beer, bool>> searchDelegate =
            x => (x.Name != null && x.Name.ToUpper().Contains(searchQuery)) ||
                 (x.Brewery != null && x.Brewery.Name != null && x.Brewery.Name.ToUpper().Contains(searchQuery)) ||
                 (x.Style != null && x.Style.ToUpper().Contains(searchQuery)) ||
                 (x.Description != null && x.Description.ToUpper().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}