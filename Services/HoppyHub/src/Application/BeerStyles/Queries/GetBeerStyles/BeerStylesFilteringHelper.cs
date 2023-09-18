using System.Linq.Expressions;
using Application.Common.Abstractions;
using Domain.Entities;

namespace Application.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     BeerStylesFilteringHelper class.
/// </summary>
public class BeerStylesFilteringHelper : FilteringHelperBase<BeerStyle, GetBeerStylesQuery>
{
    /// <summary>
    ///     Beer styles sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<BeerStyle, object>>> SortingColumns = new()
    {
        { nameof(BeerStyle.Name).ToUpper(), x => x.Name ?? string.Empty },
        { nameof(BeerStyle.CountryOfOrigin).ToUpper(), x => x.CountryOfOrigin ?? string.Empty }
    };

    /// <summary>
    ///     Initializes BeerStylesFilteringHelper.
    /// </summary>
    public BeerStylesFilteringHelper() : base(SortingColumns)
    {
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetBeerStylesQuery</param>
    public override IEnumerable<Expression<Func<BeerStyle, bool>>> GetDelegates(GetBeerStylesQuery request)
    {
        var delegates = new List<Expression<Func<BeerStyle, bool>>>();

        if (!string.IsNullOrWhiteSpace(request.CountryOfOrigin))
            delegates.Add(x =>
                x.CountryOfOrigin != null &&
                string.Equals(x.CountryOfOrigin.ToUpper(), request.CountryOfOrigin.ToUpper()));

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<BeerStyle, bool>> searchDelegate =
            x => (x.Name != null && x.Name.ToUpper().Contains(searchQuery)) ||
                 (x.Description != null && x.Description.ToUpper().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}