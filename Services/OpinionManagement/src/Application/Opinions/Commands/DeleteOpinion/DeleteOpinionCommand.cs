using MediatR;

namespace Application.Opinions.Commands.DeleteOpinion;

/// <summary>
///     DeleteOpinion command.
/// </summary>
public record DeleteOpinionCommand : IRequest
{
    /// <summary>
    ///     The opinion id.
    /// </summary>
    public Guid Id { get; init; }
}