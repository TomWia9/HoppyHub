using Application.EventConsumers;
using Application.Interfaces;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.EventConsumers;

/// <summary>
///     Tests for the <see cref="ImagesDeletedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ImagesDeletedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<ImagesDeleted>> _consumeContextMock;

    /// <summary>
    ///     The ImagesDeletedConsumer consumer.
    /// </summary>
    private readonly ImagesDeletedConsumer _consumer;

    /// <summary>
    ///     The ImagesService mock.
    /// </summary>
    private readonly Mock<IImagesService> _imagesServiceMock;

    /// <summary>
    ///     Setups ImagesDeletedConsumerTests.
    /// </summary>
    public ImagesDeletedConsumerTests()
    {
        _imagesServiceMock = new Mock<IImagesService>();
        _consumeContextMock = new Mock<ConsumeContext<ImagesDeleted>>();
        _consumer = new ImagesDeletedConsumer(_imagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method calls DeleteAllImagesInPathAsync method when message is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldCallDeleteAllImagesInPathAsyncMethod_WhenMessageIsValid()
    {
        // Arrange
        const int pathsNumber = 3;

        var message = new ImagesDeleted
        {
            Paths = Enumerable.Repeat("test/test", pathsNumber)
        };

        _consumeContextMock.Setup(x => x.Message).Returns(message);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _imagesServiceMock.Verify(x =>
            x.DeleteAllImagesInPathAsync(It.IsAny<string>()), Times.Exactly(pathsNumber));
    }
}