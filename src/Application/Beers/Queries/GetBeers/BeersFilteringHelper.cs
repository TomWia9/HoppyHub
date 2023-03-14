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
        { nameof(Beer.Brewery).ToUpper(), x => x.Brewery ?? string.Empty },
        { nameof(Beer.Style).ToUpper(), x => x.Style ?? string.Empty },
        { nameof(Beer.Country).ToUpper(), x => x.Country ?? string.Empty },
        { nameof(Beer.AlcoholByVolume).ToUpper(), x => x.AlcoholByVolume },
        { nameof(Beer.SpecificGravity).ToUpper(), x => x.SpecificGravity ?? 0 },
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
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Brewery))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Style))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Country))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Name))
            delegates.Add(x => string.Equals(x.Name, request.Name, StringComparison.OrdinalIgnoreCase));

        if (request.MinSpecificGravity != null || request.MaxSpecificGravity != null)
        {
            if(request.MinSpecificGravity != null)
                delegates.Add(x => x.SpecificGravity >= request.MinSpecificGravity);
            if(request.MaxSpecificGravity != null)
                delegates.Add(x => x.SpecificGravity <= request.MaxSpecificGravity);
        } 
        else if (request.MinBlg != null || request.MaxBlg != null)
        {
            if(request.MinBlg != null)
                delegates.Add(x => x.Blg >= request.MinBlg);
            if(request.MaxBlg != null)
                delegates.Add(x => x.Blg <= request.MaxBlg);
        }
        else if (request.MinPlato != null || request.MaxPlato != null)
        {
            if(request.MinPlato != null)
                delegates.Add(x => x.Plato >= request.MinPlato);
            if(request.MaxPlato != null)
                delegates.Add(x => x.Plato <= request.MaxPlato);
        }
        
        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();
        Expression<Func<Beer, bool>> searchDelegate =
            x => x.Description != null && x.Country != null && x.Style != null && x.Brewery != null && x.Name != null &&
                 (x.Name.ToUpper().Contains(searchQuery) || x.Brewery.ToUpper().Contains(searchQuery) ||
                  x.Style.ToUpper().Contains(searchQuery) || x.Country.ToUpper().Contains(searchQuery) ||
                  x.Description.ToUpper().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}