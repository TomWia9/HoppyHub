using Application.Opinions.Dtos;
using MediatR;

namespace Application.Opinions.Queries.GetOpinion;

/// <summary>
///     GetOpinion query.
/// </summary>
public record GetOpinionQuery : IRequest<OpinionDto>
{
    /// <summary>
    ///     The opinion id.
    /// </summary>
    public Guid Id { get; set; }
}