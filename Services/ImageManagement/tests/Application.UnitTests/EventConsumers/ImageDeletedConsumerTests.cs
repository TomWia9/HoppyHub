using Application.EventConsumers;
using Application.Interfaces;
using MassTransit;
using Moq;
using SharedEvents;

namespace Application.UnitTests.EventConsumers;

/// <summary>
///     Tests for the <see cref="ImageDeletedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImageDeletedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<ImageDeleted>> _consumeContextMock;

    /// <summary>
    ///     The ImageDeleted consumer.
    /// </summary>
    private readonly ImageDeletedConsumer _consumer;

    /// <summary>
    ///     The ImagesService mock.
    /// </summary>
    private readonly Mock<IImagesService> _imagesServiceMock;

    /// <summary>
    ///     Setups ImageDeletedConsumerTests.
    /// </summary>
    public ImageDeletedConsumerTests()
    {
        _imagesServiceMock = new Mock<IImagesService>();
        _consumeContextMock = new Mock<ConsumeContext<ImageDeleted>>();
        _consumer = new ImageDeletedConsumer(_imagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method calls DeleteImageAsync method when message is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldCallDeleteImageAsyncMethod_WhenMessageIsValid()
    {
        // Arrange
        var message = new ImageDeleted
        {
            Uri = "https://test.com/test.jpg"
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _imagesServiceMock.Verify(x =>
            x.DeleteImageAsync(message.Uri), Times.Once());
    }
}