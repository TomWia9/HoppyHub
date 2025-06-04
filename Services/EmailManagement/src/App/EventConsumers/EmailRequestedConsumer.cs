using App.Interfaces;
using MassTransit;
using SharedEvents.Events;

namespace App.EventConsumers;

/// <summary>
///     EmailRequested consumer.
/// </summary>
public class EmailRequestedConsumer : IConsumer<EmailRequested>
{
    /// <summary>
    ///     The email sender.
    /// </summary>
    private readonly IEmailSender _emailSender;

    /// <summary>
    ///     Initializes EmailRequestedConsumer.
    /// </summary>
    /// <param name="emailSender">The email sender.</param>
    public EmailRequestedConsumer(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    /// <summary>
    ///     Consumes EmailRequested event.
    /// </summary>
    /// <param name="context">The consume context</param>
    public async Task Consume(ConsumeContext<EmailRequested> context)
    {
        var message = context.Message;

        await _emailSender.SendEmailAsync(message);
    }
}