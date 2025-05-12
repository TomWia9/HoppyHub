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
    /// <param name="sendEmailRequestedEvent">The emailSent event.</param>
    Task SendEmailAsync(SendEmailRequested sendEmailRequestedEvent);
}