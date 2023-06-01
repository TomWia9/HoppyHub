using MediatR;

namespace Application.Opinions.Commands.UpdateOpinion;

/// <summary>
///     UpdateOpinion command.
/// </summary>
public record UpdateOpinionCommand : IRequest
{   
    /// <summary>
    ///     The opinion id.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    ///     The rating in 1-10 scale.
    /// </summary>
    public int Rating { get; init; }

    /// <summary>
    ///     The comment.
    /// </summary>
    public string? Comment { get; init; }
}