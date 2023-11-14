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
///     Unit tests for the <see cref="BeerImageUploadedToBlobStorageConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerImageUploadedToBlobStorageConsumerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerImageUploadedToBlobStorage>> _consumeContextMock;

    /// <summary>
    ///     The BeerImageUploadedToBlobStorage consumer. 
    /// </summary>
    private readonly BeerImageUploadedToBlobStorageConsumer _consumer;

    /// <summary>
    ///     Setups BeerImageUploadedToBlobStorageConsumerTests.
    /// </summary>
    public BeerImageUploadedToBlobStorageConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerImageUploadedToBlobStorage>>();
        _consumer = new BeerImageUploadedToBlobStorageConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method adds beer image to database when beer exists and beer image does not exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldAddBeerImageToDatabase_WhenBeerExistsAndBeerImageDoesNotExists()
    {
        // Arrange
        const string imageUri = "https://test.blob.core.windows.net/test-container/Beers/image.jpg";
        var beerId = Guid.NewGuid();
        var message = new BeerImageUploadedToBlobStorage
        {
            BeerId = beerId,
            ImageUri = imageUri,
        };
        var beer = new Beer
        {
            Id = beerId
        };
        var beerImages = Enumerable.Empty<BeerImage>();
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Consume method updates beer image when beer and beer image exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldUpdateBeerImage_WhenBeerAndBeerImageExits()
    {
        // Arrange
        const string imageUri = "https://test.blob.core.windows.net/test-container/Beers/image.jpg";
        var beerId = Guid.NewGuid();
        var message = new BeerImageUploadedToBlobStorage
        {
            BeerId = beerId,
            ImageUri = imageUri,
        };
        var beer = new Beer
        {
            Id = beerId
        };
        var beerImage = new BeerImage
        {
            Id = Guid.NewGuid(),
            BeerId = beerId,
            ImageUri = "uriBeforeUpdate",
            TempImage = false
        };
        var beerImages = new List<BeerImage> { beerImage };
        var beerImagesDbSetMock = beerImages.AsQueryable().BuildMockDbSet();

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.BeerImages).Returns(beerImagesDbSetMock.Object);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.BeerImages.AddAsync(It.IsAny<BeerImage>(), CancellationToken.None), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Consume method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldThrowNotFoundException_WhenBeerDoesNotExists()
    {
        // Arrange
        const string imageUri = "https://test.blob.core.windows.net/test-container/Beers/image.jpg";
        var beerId = Guid.NewGuid();
        var message = new BeerImageUploadedToBlobStorage
        {
            BeerId = beerId,
            ImageUri = imageUri,
        };

        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _consumer.Invoking(x => x.Consume(_consumeContextMock.Object))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}