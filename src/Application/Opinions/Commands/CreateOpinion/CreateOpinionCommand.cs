using Application.Opinions.Commands.Common;
using Application.Opinions.Dtos;
using MediatR;

namespace Application.Opinions.Commands.CreateOpinion;

/// <summary>
///     CreateOpinion command.
/// </summary>
public record CreateOpinionCommand : BaseOpinionCommand, IRequest<OpinionDto>
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }
}