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
    ///     The logger.
    /// </summary>
    private readonly ILogger<EmailSender> _logger;

    /// <summary>
    ///     Initializes EmailSender.
    /// </summary>
    public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
    {
        _logger = logger;
        _emailClient = new EmailClient(config["CommunicationServices:ConnectionString"]);
        _sender = config["CommunicationServices:EmailSender"];
    }

    public async Task SendEmailAsync(EmailRequested emailRequestedEvent)
    {
        var emailContent = new EmailContent(emailRequestedEvent.Subject)
        {
            Html = emailRequestedEvent.Content
        };
        var emailRecipients = new EmailRecipients(new[] { new EmailAddress(emailRequestedEvent.Recipient) });
        var emailMessage = new EmailMessage(_sender, emailRecipients, emailContent);

        try
        {
            var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            var operationId = emailSendOperation.Id;

            _logger.LogInformation("Email Sent. Status = {ValueStatus}", emailSendOperation.Value.Status);
            _logger.LogInformation("Email operation id = {OperationId}", operationId);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Email send operation failed with error code: {ExErrorCode}, message: {ExMessage}",
                ex.ErrorCode,
                ex.Message);
        }
    }
}