using Application.EventConsumers;
using Application.Interfaces;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedEvents.Responses;

namespace Application.UnitTests.EventConsumers;

/// <summary>
///     Tests for the <see cref="ImageCreatedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImageCreatedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<ImageCreated>> _consumeContextMock;

    /// <summary>
    ///     The ImageCreated consumer.
    /// </summary>
    private readonly ImageCreatedConsumer _consumer;

    /// <summary>
    ///     The ImagesService mock.
    /// </summary>
    private readonly Mock<IImagesService> _imagesServiceMock;

    /// <summary>
    ///     Setups ImageCreatedConsumerTests.
    /// </summary>
    public ImageCreatedConsumerTests()
    {
        _imagesServiceMock = new Mock<IImagesService>();
        _consumeContextMock = new Mock<ConsumeContext<ImageCreated>>();
        _consumer = new ImageCreatedConsumer(_imagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method calls UploadImageAsync method when message is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldCallUploadImageAsyncMethodAndReturnImageUri_WhenMessageIsValid()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
        var message = new ImageCreated
        {
            Path = "test/test",
            Image = new byte[1]
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _imagesServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
            .ReturnsAsync(imageUri);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _imagesServiceMock.Verify(x =>
            x.UploadImageAsync(message.Path, message.Image), Times.Once());
        _consumeContextMock.Verify(x => x.RespondAsync(It.Is<ImageUploaded>(y => y.Uri == imageUri)), Times.Once);
    }
}