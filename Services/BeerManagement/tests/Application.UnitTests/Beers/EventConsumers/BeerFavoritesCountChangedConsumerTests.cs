using Application.Beers.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Beers.EventConsumers;

/// <summary>
///     Tests for the <see cref="BeerFavoritesCountChangedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerFavoritesCountChangedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerFavoritesCountChanged>> _consumeContextMock;

    /// <summary>
    ///     The FavoritesCountChanged consumer.
    /// </summary>
    private readonly BeerFavoritesCountChangedConsumer _consumer;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     Setups BeerCreatedConsumerTests.
    /// </summary>
    public BeerFavoritesCountChangedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerFavoritesCountChanged>>();

        _consumer = new BeerFavoritesCountChangedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method updates beer FavoritesCount message is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldUpdateBeerFavoritesCount_WhenMessageIsValid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            FavoritesCount = 0
        };
        var message = new BeerFavoritesCountChanged
        {
            BeerId = beerId,
            FavoritesCount = 1
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId)).ReturnsAsync(beer);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        beer.FavoritesCount.Should().Be(message.FavoritesCount);
        _contextMock.Verify(x => x.Beers.FindAsync(beerId), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}