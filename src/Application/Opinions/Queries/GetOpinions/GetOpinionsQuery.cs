using Application.Common.Abstractions;
using Application.Common.Models;
using Application.Opinions.Dtos;
using MediatR;

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
    ///     Indicates whether opinions has images
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