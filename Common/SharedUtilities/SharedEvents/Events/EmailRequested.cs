namespace SharedEvents.Events;

/// <summary>
///     The email requested event.
/// </summary>
public record EmailRequested
{
    /// <summary>
    ///     The recipient.
    /// </summary>
    public string? Recipient { get; init; }

    /// <summary>
    ///     The subject.
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    ///     The content.
    /// </summary>
    public string? Content { get; init; }
}