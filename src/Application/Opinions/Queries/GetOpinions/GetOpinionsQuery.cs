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
    ///     Minimum rate.
    /// </summary>
    public int? MinRate { get; init; } = 1;

    /// <summary>
    ///     Maximum rate.
    /// </summary>
    public int? MaxRate { get; init; } = 10;

    /// <summary>
    ///     Beer id.
    /// </summary>
    public Guid? BeerId { get; init; }
    
    /// <summary>
    ///     User id.
    /// </summary>
    public Guid? UserId { get; init; }
}