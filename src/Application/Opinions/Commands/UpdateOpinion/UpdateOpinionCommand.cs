using Application.Opinions.Commands.Common;
using MediatR;

namespace Application.Opinions.Commands.UpdateOpinion;

/// <summary>
///     UpdateOpinion command.
/// </summary>
public record UpdateOpinionCommand : BaseOpinionCommand, IRequest
{   
    /// <summary>
    ///     The opinion id.
    /// </summary>
    public Guid Id { get; init; }
}