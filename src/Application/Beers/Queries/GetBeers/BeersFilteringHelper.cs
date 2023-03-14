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
        { nameof(Beer.Name).ToLower(), x => x.Name ?? string.Empty },
        { nameof(Beer.Brewery).ToLower(), x => x.Brewery ?? string.Empty },
        { nameof(Beer.Style).ToLower(), x => x.Style ?? string.Empty },
        { nameof(Beer.Country).ToLower(), x => x.Country ?? string.Empty },
        { nameof(Beer.AlcoholByVolume).ToLower(), x => x.AlcoholByVolume },
        { nameof(Beer.SpecificGravity).ToLower(), x => x.SpecificGravity ?? 0 },
        { nameof(Beer.Blg).ToLower(), x => x.Blg ?? 0 },
        { nameof(Beer.Plato).ToLower(), x => x.Plato ?? 0 },
        { nameof(Beer.Ibu).ToLower(), x => x.Ibu ?? 0 }
    };

    /// <summary>
    ///     Gets sorting column expression.
    /// </summary>
    /// <param name="sortBy">Column by which to sort</param>
    /// <returns>The sorting expression</returns>
    public static Expression<Func<Beer, object>> GetSortingColumn(string? sortBy)
    {
        return string.IsNullOrEmpty(sortBy) ? SortingColumns.First().Value : SortingColumns[sortBy.ToLower()];
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
            x => x.SpecificGravity >= request.MinSpecificGravity && x.SpecificGravity <= request.MaxSpecificGravity,
            x => x.Blg >= request.MinBlg && x.Blg <= request.MaxBlg,
            x => x.Plato >= request.MinPlato && x.Plato <= request.MaxPlato,
            x => x.Ibu >= request.MinIbu && x.Ibu <= request.MaxIbu,
        };

        if (!string.IsNullOrWhiteSpace(request.Name))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Brewery))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Style))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Country))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Name))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToLower();
        Expression<Func<Beer, bool>> searchDelegate =
            x => x.Description != null && x.Country != null && x.Style != null && x.Brewery != null && x.Name != null &&
                 (x.Name.ToLower().Contains(searchQuery) || x.Brewery.ToLower().Contains(searchQuery) ||
                  x.Style.ToLower().Contains(searchQuery) || x.Country.ToLower().Contains(searchQuery) ||
                  x.Description.ToLower().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}