﻿using System.Linq.Expressions;
using Domain.Entities;
using SharedUtilities.Abstractions;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     OpinionsFilteringHelper class.
/// </summary>
public class OpinionsFilteringHelper : FilteringHelperBase<Opinion, GetOpinionsQuery>
{
    /// <summary>
    ///     Opinions sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Opinion, object>>> SortingColumns = new()
    {
        { nameof(Opinion.LastModified).ToUpper(), x => x.LastModified ?? new DateTimeOffset() },
        { nameof(Opinion.Created).ToUpper(), x => x.Created ?? new DateTimeOffset() },
        { nameof(Opinion.Rating).ToUpper(), x => x.Rating },
        { nameof(Opinion.Comment).ToUpper(), x => x.Comment ?? string.Empty }
    };

    /// <summary>
    ///     Initializes OpinionsFilteringHelper.
    /// </summary>
    public OpinionsFilteringHelper() : base(SortingColumns)
    {
    }

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetOpinionsQuery</param>
    public override IEnumerable<Expression<Func<Opinion, bool>>> GetDelegates(GetOpinionsQuery request)
    {
        var delegates = new List<Expression<Func<Opinion, bool>>>
        {
            x => x.Rating >= request.MinRating && x.Rating <= request.MaxRating,
            x => x.Created != null && x.Created.Value >= request.From && x.Created.Value <= request.To
        };

        if (request.BeerId is not null)
            delegates.Add(x => x.BeerId == request.BeerId);

        if (request.UserId is not null)
            delegates.Add(x => x.CreatedBy == request.UserId);

        if (request.HasImage is not null)
            delegates.Add(x => !string.IsNullOrEmpty(x.ImageUri) == request.HasImage);

        if (request.HasComment is not null)
            delegates.Add(x => !string.IsNullOrEmpty(x.Comment) == request.HasComment);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Opinion, bool>> searchDelegate =
            x => (x.Beer != null && x.Beer.Name != null && x.Beer.Name.ToUpper().Contains(searchQuery)) ||
                 (x.User != null && !string.IsNullOrEmpty(x.User.Username) &&
                  x.User.Username.ToUpper().Contains(searchQuery));

        delegates.Add(searchDelegate);

        return delegates;
    }
}