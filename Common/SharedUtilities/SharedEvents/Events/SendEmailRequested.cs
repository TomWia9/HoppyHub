namespace SharedEvents.Events;

/// <summary>
///     The send email requested event.
/// </summary>
public record SendEmailRequested
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