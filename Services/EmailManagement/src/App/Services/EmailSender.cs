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
    ///     Initializes EmailSender.
    /// </summary>
    public EmailSender(IConfiguration config)
    {
        _emailClient = new EmailClient(config["CommunicationServices:ConnectionString"]);
    }

    public async Task SendEmailAsync(EmailSent emailSentEvent)
    {
        //Dev purpose
        var sender = ""; //have to be connected domain in azure
        emailSentEvent = new EmailSent
        {
            Subject = "EmailManagement",
            Content =
                "<html><body><h1>Quick send email test</h1><br/><h4>This email message is sent from Azure Communication Service Email.</h4><p>This mail was sent using .NET SDK!!</p></body></html>",
            Receiver = ""
        };
        //

        var emailContent = new EmailContent(emailSentEvent.Subject)
        {
            Html = emailSentEvent.Content
        };
        var emailRecipients = new EmailRecipients(new[] { new EmailAddress(emailSentEvent.Receiver) });
        var emailMessage = new EmailMessage(sender, emailRecipients, emailContent);

        await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
    }
}