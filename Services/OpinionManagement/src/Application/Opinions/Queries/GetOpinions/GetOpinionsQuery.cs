using Application.Opinions.Dtos;
using MediatR;
using SharedUtilities.Abstractions;
using SharedUtilities.Models;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     GetOpinions query.
/// </summary>
public record GetOpinionsQuery : QueryParameters, IRequest<PaginatedList<OpinionDto>>
{
    /// <summary>
    ///     Minimum rating.
    /// </summary>
    public int? MinRating { get; init; } = 1;

    /// <summary>
    ///     Maximum rating.
    /// </summary>
    public int? MaxRating { get; init; } = 10;

    /// <summary>
    ///     Minimum created date.
    /// </summary>
    public DateTime From { get; init; } = DateTime.MinValue;

    /// <summary>
    ///     Maximum created date.
    /// </summary>
    public DateTime To { get; init; } = DateTime.Now;

    /// <summary>
    ///     Indicates whether opinions have images.
    /// </summary>
    public bool? HaveImages { get; init; }

    /// <summary>
    ///     Beer id.
    /// </summary>
    public Guid? BeerId { get; init; }

    /// <summary>
    ///     User id.
    /// </summary>
    public Guid? UserId { get; init; }
}