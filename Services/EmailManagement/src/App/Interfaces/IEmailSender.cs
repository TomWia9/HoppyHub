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
    /// <param name="emailRequestedEvent">The EmailRequested event.</param>
    Task SendEmailAsync(EmailRequested emailRequestedEvent);
}