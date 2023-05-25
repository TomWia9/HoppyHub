using Application.Opinions.Dtos;
using MediatR;

namespace Application.Opinions.Commands.CreateOpinion;

/// <summary>
///     CreateOpinion command.
/// </summary>
public record CreateOpinionCommand : IRequest<OpinionDto>
{
    /// <summary>
    ///     The rate in 1-10 scale.
    /// </summary>
    public int Rate { get; init; }

    /// <summary>
    ///     The comment.
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }
}