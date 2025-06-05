using App.Interfaces;
using App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace App.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="EmailSender" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class EmailSenderTests
{
    /// <summary>
    ///     The email sender.
    /// </summary>
    private readonly IEmailSender _emailSender;

    /// <summary>
    ///     The configuration mock.
    /// </summary>
    private readonly Mock<IConfiguration> _configurationMock;

    /// <summary>
    ///     Setups EmailSenderTests.
    /// </summary>
    public EmailSenderTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        Mock<ILogger<EmailSender>> loggerMock = new();
        _configurationMock.Setup(x => x["CommunicationServices:EmailSender"])
            .Returns("sender@example.com");
        _configurationMock.Setup(x => x["CommunicationServices:ConnectionString"])
            .Returns("Endpoint=https://test.com;AccessKey=testkey");

        _emailSender = new EmailSender(_configurationMock.Object, loggerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Act & Assert
        _emailSender.Should().NotBeNull();
        _configurationMock.Verify(x => x["CommunicationServices:ConnectionString"], Times.Once);
        _configurationMock.Verify(x => x["CommunicationServices:EmailSender"], Times.Once);
    }
}