namespace Application.Opinions.Commands.Common;

/// <summary>
///     BaseOpinionCommand abstract record.
/// </summary>
public abstract record BaseOpinionCommand
{
    /// <summary>
    ///     The rating in 1-10 scale.
    /// </summary>
    public int Rating { get; init; }

    /// <summary>
    ///     The comment.
    /// </summary>
    public string? Comment { get; init; }
}