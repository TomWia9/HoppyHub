using Application.Beers.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;

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
    ///     The BeerDeleted consumer.
    /// </summary>
    private readonly BeerDeletedConsumer _consumer;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     Setups BeerDeletedConsumerTests.
    /// </summary>
    public BeerDeletedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerDeleted>>();

        _consumer = new BeerDeletedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method deleted beer from database when message id is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldDeleteBeerFromDatabase_WhenMessageIsValid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var existingBeer = new Beer
        {
            Id = beerId,
            Favorites = new List<Favorite>()
        };
        var message = new BeerDeleted
        {
            Id = beerId
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId)).ReturnsAsync(existingBeer);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        _contextMock.Verify(x => x.Beers.Remove(It.IsAny<Beer>()), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}