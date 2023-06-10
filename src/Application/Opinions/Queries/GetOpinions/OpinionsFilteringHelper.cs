using System.Linq.Expressions;
using Application.Common.Abstractions;
using Domain.Entities;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     OpinionsFilteringHelper class.
/// </summary>
public class OpinionsFilteringHelper : FilteringHelperBase<Opinion, GetOpinionsQuery>
{
    /// <summary>
    ///     Initializes OpinionsFilteringHelper.
    /// </summary>
    public OpinionsFilteringHelper() : base(SortingColumns)
    {
    }

    /// <summary>
    ///     Opinions sorting columns.
    /// </summary>
    public static readonly Dictionary<string, Expression<Func<Opinion, object>>> SortingColumns = new()
    {
        { nameof(Opinion.LastModified).ToUpper(), x => x.LastModified ?? new DateTime() },
        { nameof(Opinion.Rating).ToUpper(), x => x.Rating },
        { nameof(Opinion.Comment).ToUpper(), x => x.Comment ?? string.Empty }
    };

    /// <summary>
    ///     Gets filtering and searching delegates.
    /// </summary>
    /// <param name="request">The GetOpinionsQuery</param>
    public override IEnumerable<Expression<Func<Opinion, bool>>> GetDelegates(GetOpinionsQuery request)
    {
        var delegates = new List<Expression<Func<Opinion, bool>>>
        {
            x => x.Rating >= request.MinRating && x.Rating <= request.MaxRating
        };

        if (request.BeerId != null)
            delegates.Add(x => x.BeerId == request.BeerId);

        if (request.UserId != null)
            delegates.Add(x => x.CreatedBy == request.UserId);

        if (request.HaveImages != null)
            delegates.Add(x => !string.IsNullOrEmpty(x.ImageUri) == request.HaveImages);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            return delegates;
        }

        var searchQuery = request.SearchQuery.Trim().ToUpper();

        Expression<Func<Opinion, bool>> searchDelegate =
            x => x.Comment != null && x.Comment.ToUpper().Contains(searchQuery);

        delegates.Add(searchDelegate);

        return delegates;
    }
}