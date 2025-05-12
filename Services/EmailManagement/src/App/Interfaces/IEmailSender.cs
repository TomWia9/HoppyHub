using SharedEvents.Events;

namespace App.Interfaces;

/// <summary>
///     Email sender interface.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    ///     Sends email async.
    /// </summary>
    /// <param name="emailSentEvent">The emailSent event.</param>
    Task SendEmailAsync(EmailSent emailSentEvent);
}