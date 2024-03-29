﻿using System.Linq.Expressions;
using Domain.Entities;
using SharedUtilities.Abstractions;

namespace Application.Breweries.Queries.GetBreweries;

/// <summary>
///     BreweriesFilteringHelper class.
/// </summary>
public class BreweriesFilteringHelper : FilteringHelperBase<Brewery, GetBreweriesQuery>
{
    /// <summary>
    ///     Breweries sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Brewery, object>>> SortingColumns = new()
    {
        { nameof(Brewery.Name).ToUpper(), x => x.Name ?? string.Empty },
        { nameof(Brewery.FoundationYear).ToUpper(), x => x.FoundationYear }
    };

    /// <summary>
    ///     Initializes BreweriesFilteringHelper.
    /// </summary>
    public BreweriesFilteringHelper() : base(SortingColumns)
    {
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetBreweriesQuery</param>
    public override IEnumerable<Expression<Func<Brewery, bool>>> GetDelegates(GetBreweriesQuery request)
    {
        var delegates = new List<Expression<Func<Brewery, bool>>>
        {
            x => x.FoundationYear >= request.MinFoundationYear && x.FoundationYear <= request.MaxFoundationYear
        };

        if (!string.IsNullOrWhiteSpace(request.Name))
            delegates.Add(x => x.Name != null && string.Equals(x.Name.ToUpper(), request.Name.ToUpper()));

        if (!string.IsNullOrWhiteSpace(request.Country))
            delegates.Add(x =>
                x.Address != null && x.Address.Country != null &&
                string.Equals(x.Address.Country.ToUpper(), request.Country.ToUpper()));

        if (!string.IsNullOrWhiteSpace(request.State))
            delegates.Add(x =>
                x.Address != null && x.Address.State != null &&
                string.Equals(x.Address.State.ToUpper(), request.State.ToUpper()));

        if (!string.IsNullOrWhiteSpace(request.City))
            delegates.Add(x =>
                x.Address != null && x.Address.City != null &&
                string.Equals(x.Address.City.ToUpper(), request.City.ToUpper()));

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Brewery, bool>> searchDelegate =
            x => x.Name != null && x.Name.ToUpper().Contains(searchQuery);

        delegates.Add(searchDelegate);

        return delegates;
    }
}