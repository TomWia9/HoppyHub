using App.EventConsumers;
using App.Interfaces;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace App.UnitTests.EventConsumers;

/// <summary>
///     Tests for the <see cref="EmailRequestedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class EmailRequestedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<EmailRequested>> _consumeContextMock;

    /// <summary>
    ///     The email sender mock.
    /// </summary>
    private readonly Mock<IEmailSender> _emailSenderMock;

    /// <summary>
    ///     The EmailRequested consumer.
    /// </summary>
    private readonly EmailRequestedConsumer _consumer;

    /// <summary>
    ///     Setups EmailRequestedConsumer.
    /// </summary>
    public EmailRequestedConsumerTests()
    {
        _consumeContextMock = new Mock<ConsumeContext<EmailRequested>>();
        _emailSenderMock = new Mock<IEmailSender>();
        _consumer = new EmailRequestedConsumer(_emailSenderMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method calls email sender when message is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldCallEmailSender_WhenMessageIsValid()
    {
        // Arrange
        var message = new EmailRequested
        {
            Recipient = "test@localhost",
            Subject = "Test subject",
            Content = "Test content"
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _emailSenderMock.Verify(x => x.SendEmailAsync(message), Times.Once);
    }
}