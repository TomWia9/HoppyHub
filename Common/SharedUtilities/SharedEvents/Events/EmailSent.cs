namespace SharedEvents.Events;

/// <summary>
///     The email sent event.
/// </summary>
public record EmailSent
{
    /// <summary>
    ///     The receiver.
    /// </summary>
    public string? Receiver { get; init; }

    /// <summary>
    ///     The subject.
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    ///     The content.
    /// </summary>
    public string? Content { get; init; }
}