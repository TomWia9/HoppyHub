using Application.Beers.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Beers.EventConsumers;

/// <summary>
///     Tests for the <see cref="BeerDeletedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerDeletedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerDeleted>> _consumeContextMock;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The storage container service mock.
    /// </summary>
    private readonly Mock<IStorageContainerService> _storageContainerServiceMock;

    /// <summary>
    ///     The BeerDeleted consumer.
    /// </summary>
    private readonly BeerDeletedConsumer _consumer;

    /// <summary>
    ///     Setups BeerDeletedConsumerTests.
    /// </summary>
    public BeerDeletedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerDeleted>>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();

        _consumer = new BeerDeletedConsumer(_contextMock.Object, _storageContainerServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method deleted beer from database and related beer opinion images from storage container when message id is valid.
    /// </summary>
    [Fact]
    public async Task
        Consume_ShouldDeleteBeerFromDatabaseAndRelatedBeerOpinionImagesFromStorageContainer_WhenMessageIsValid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var existingBeer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var message = new BeerDeleted
        {
            Id = beerId
        };
        var expectedPathToDelete = $"Opinions/{existingBeer.BreweryId}/{existingBeer.Id}";
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId)).ReturnsAsync(existingBeer);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Beers.Remove(It.IsAny<Beer>()), Times.Once);
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(expectedPathToDelete), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}