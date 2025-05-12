using App.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace App.EventConsumers;

/// <summary>
///     EmailSent consumer.
/// </summary>
public class EmailSentConsumer : IConsumer<EmailSent>
{
    /// <summary>
    ///     The email sender.
    /// </summary>
    private readonly IEmailSender _emailSender;

    /// <summary>
    ///     Initializes EmailSentConsumer.
    /// </summary>
    /// <param name="emailSender">The email sender.</param>
    public EmailSentConsumer(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    /// <summary>
    ///     Consumes BeerOpinionChanged event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<EmailSent> context)
    {
        var message = context.Message;

        await _emailSender.SendEmailAsync(message);
    }
}