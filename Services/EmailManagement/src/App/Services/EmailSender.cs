using App.Interfaces;
using Azure;
using Azure.Communication.Email;
using SharedEvents.Events;

namespace App.Services;

/// <summary>
///     The email sender.
/// </summary>
public class EmailSender : IEmailSender
{
    /// <summary>
    ///     The email client.
    /// </summary>
    private readonly EmailClient _emailClient;

    /// <summary>
    ///     The email sender.
    /// </summary>
    private readonly string? _sender;

    /// <summary>
    ///     Initializes EmailSender.
    /// </summary>
    public EmailSender(IConfiguration config)
    {
        _emailClient = new EmailClient(config["CommunicationServices:ConnectionString"]);
        _sender = config["CommunicationServices:EmailSender"];
    }

    public async Task SendEmailAsync(SendEmailRequested sendEmailRequestedEvent)
    {
        //Dev purpose
        sendEmailRequestedEvent = new SendEmailRequested
        {
            Subject = "EmailManagement",
            Content =
                "<html><body><h1>Quick send email test</h1><br/><h4>This email message is sent from Azure Communication Service Email.</h4><p>This mail was sent using .NET SDK!!</p></body></html>",
            Recipient = ""
        };
        //

        var emailContent = new EmailContent(sendEmailRequestedEvent.Subject)
        {
            Html = sendEmailRequestedEvent.Content
        };
        var emailRecipients = new EmailRecipients(new[] { new EmailAddress(sendEmailRequestedEvent.Recipient) });
        var emailMessage = new EmailMessage(_sender, emailRecipients, emailContent);

        await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
    }
}