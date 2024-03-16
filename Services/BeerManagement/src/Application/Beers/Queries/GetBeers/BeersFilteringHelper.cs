using System.Linq.Expressions;
using Domain.Entities;
using SharedUtilities.Abstractions;

namespace Application.Beers.Queries.GetBeers;

/// <summary>
///     BeersFilteringHelper class.
/// </summary>
public class BeersFilteringHelper : FilteringHelperBase<Beer, GetBeersQuery>
{
    /// <summary>
    ///     Beers sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Beer, object>>> SortingColumns = new()
    {
        { nameof(Beer.Name).ToUpper(), x => x.Name ?? string.Empty },
        { nameof(Beer.Brewery).ToUpper(), x => x.Brewery!.Name ?? string.Empty },
        { nameof(Beer.BeerStyle).ToUpper(), x => x.BeerStyle!.Name ?? string.Empty },
        { nameof(Beer.AlcoholByVolume).ToUpper(), x => x.AlcoholByVolume },
        { nameof(Beer.Blg).ToUpper(), x => x.Blg ?? 0 },
        { nameof(Beer.Ibu).ToUpper(), x => x.Ibu ?? 0 },
        { nameof(Beer.Rating).ToUpper(), x => x.Rating },
        { nameof(Beer.OpinionsCount).ToUpper(), x => x.OpinionsCount },
        { nameof(Beer.FavoritesCount).ToUpper(), x => x.FavoritesCount },
        { nameof(Beer.ReleaseDate).ToUpper(), x => x.ReleaseDate ?? new DateOnly() }
    };

    /// <summary>
    ///     Initializes BeersFilteringHelper.
    /// </summary>
    public BeersFilteringHelper() : base(SortingColumns)
    {
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetBeersQuery</param>
    public override IEnumerable<Expression<Func<Beer, bool>>> GetDelegates(GetBeersQuery request)
    {
        var delegates = new List<Expression<Func<Beer, bool>>>
        {
            x => x.AlcoholByVolume >= request.MinAlcoholByVolume && x.AlcoholByVolume <= request.MaxAlcoholByVolume,
            x => x.Rating >= request.MinRating && x.Rating <= request.MaxRating,
            x => x.OpinionsCount >= request.MinOpinionsCount && x.OpinionsCount <= request.MaxOpinionsCount,
            x => x.FavoritesCount >= request.MinFavoritesCount && x.FavoritesCount <= request.MaxFavoritesCount,
            x => (x.Blg >= request.MinExtract && x.Blg <= request.MaxExtract) || x.Blg == null,
            x => (x.Ibu >= request.MinIbu && x.Ibu <= request.MaxIbu) || x.Ibu == null,
            x => (x.ReleaseDate != null && x.ReleaseDate.Value >= DateOnly.Parse(request.MinReleaseDate) &&
                  x.ReleaseDate.Value <= DateOnly.Parse(request.MaxReleaseDate)) || x.ReleaseDate == null
        };

        if (!string.IsNullOrWhiteSpace(request.Name))
            delegates.Add(x => x.Name != null && string.Equals(x.Name.ToUpper(), request.Name.ToUpper()));

        if (request.BreweryId is not null)
            delegates.Add(x => x.Brewery != null && x.Brewery.Id == request.BreweryId);

        if (request.BeerStyleId is not null)
            delegates.Add(x => x.BeerStyle != null && x.BeerStyle.Id == request.BeerStyleId);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Beer, bool>> searchDelegate =
            x => (x.Name != null && x.Name.ToUpper().Contains(searchQuery)) ||
                 (x.Brewery != null && x.Brewery.Name != null && x.Brewery.Name.ToUpper().Contains(searchQuery)) ||
                 (x.BeerStyle != null && x.BeerStyle.Name != null &&
                  x.BeerStyle.Name.ToUpper().Contains(searchQuery)) ||
                 (x.Description != null && x.Description.ToUpper().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}