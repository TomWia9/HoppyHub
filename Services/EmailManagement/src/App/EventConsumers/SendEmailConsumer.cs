using App.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace App.EventConsumers;

/// <summary>
///     SendEmail consumer.
/// </summary>
public class SendEmailConsumer : IConsumer<SendEmailRequested>
{
    /// <summary>
    ///     The email sender.
    /// </summary>
    private readonly IEmailSender _emailSender;

    /// <summary>
    ///     Initializes EmailSentConsumer.
    /// </summary>
    /// <param name="emailSender">The email sender.</param>
    public SendEmailConsumer(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    /// <summary>
    ///     Consumes SendEmailRequested event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<SendEmailRequested> context)
    {
        var message = context.Message;

        await _emailSender.SendEmailAsync(message);
    }
}