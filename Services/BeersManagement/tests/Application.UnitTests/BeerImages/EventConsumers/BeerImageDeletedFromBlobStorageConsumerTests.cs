using Application.BeerImages.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MockQueryable.Moq;
using Moq;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.BeerImages.EventConsumers;

/// <summary>
///     Unit tests for the <see cref="BeerImageDeletedFromBlobStorageConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerImageDeletedFromBlobStorageConsumerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The app configuration.
    /// </summary>
    private readonly Mock<IAppConfiguration> _appConfigurationMock;

    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerImageDeletedFromBlobStorage>> _consumeContextMock;

    /// <summary>
    ///     The BeerImageDeletedFromBlobStorage consumer. 
    /// </summary>
    private readonly BeerImageDeletedFromBlobStorageConsumer _consumer;

    /// <summary>
    ///     Setups BeerImageDeletedFromBlobStorageConsumerTests.
    /// </summary>
    public BeerImageDeletedFromBlobStorageConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _appConfigurationMock = new Mock<IAppConfiguration>();
        _consumeContextMock = new Mock<ConsumeContext<BeerImageDeletedFromBlobStorage>>();
        _consumer = new BeerImageDeletedFromBlobStorageConsumer(_contextMock.Object, _appConfigurationMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method changes beer image to temp when beer and beer image exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldChangeBeerImageToTemp_WhenBeerAndBeerImageExists()
    {
        // Arrange
        const string tempImageUri = "https://test.blob.core.windows.net/test-container/Beers/image.jpg";
        var beerId = Guid.NewGuid();
        var message = new BeerImageDeletedFromBlobStorage
        {
            BeerId = beerId
        };
        var beer = new Beer
        {
            Id = beerId,
            BeerImage = new BeerImage
            {
                Id = Guid.NewGuid(),
                BeerId = beerId,
                ImageUri = "uriBeforeUpdate",
                TempImage = false
            }
        };
        var beers = new List<Beer> { beer };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _appConfigurationMock.SetupGet(x => x.TempBeerImageUri).Returns(tempImageUri);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Consume method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldThrowNotFoundException_WhenBeerDoesNotExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var message = new BeerImageDeletedFromBlobStorage
        {
            BeerId = beerId
        };

        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _consumer.Invoking(x => x.Consume(_consumeContextMock.Object))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}