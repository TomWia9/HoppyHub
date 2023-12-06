using Application.Beers.EventConsumers;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Moq;
using SharedEvents.Events;

namespace Application.UnitTests.Beers.EventConsumers;

/// <summary>
///     Tests for the <see cref="BeerUpdatedConsumer" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerUpdatedConsumerTests
{
    /// <summary>
    ///     The consume context mock.
    /// </summary>
    private readonly Mock<ConsumeContext<BeerUpdated>> _consumeContextMock;

    /// <summary>
    ///     The application db context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The BeerUpdated consumer.
    /// </summary>
    private readonly BeerUpdatedConsumer _consumer;

    /// <summary>
    ///     Setups BeerUpdatedConsumerTests.
    /// </summary>
    public BeerUpdatedConsumerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _consumeContextMock = new Mock<ConsumeContext<BeerUpdated>>();

        _consumer = new BeerUpdatedConsumer(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Consume method updates beer in database when message id is valid.
    /// </summary>
    [Fact]
    public async Task Consume_ShouldUpdateBeerInDatabase_WhenMessageIsValid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var existingBeer = new Beer
        {
            Id = beerId,
            Name = "Old name",
            BreweryName = "Old brewery name"
        };
        var message = new BeerUpdated
        {
            Id = beerId,
            Name = "new beer name",
            BreweryId = Guid.NewGuid(),
            BreweryName = "new brewery name"
        };
        _consumeContextMock.Setup(x => x.Message).Returns(message);
        _contextMock.Setup(x => x.Beers.FindAsync(beerId)).ReturnsAsync(existingBeer);

        // Act
        await _consumer.Consume(_consumeContextMock.Object);

        // Assert
        existingBeer.Name.Should().Be(message.Name);
        existingBeer.BreweryId.Should().Be(message.BreweryId);
        existingBeer.BreweryName.Should().Be(message.BreweryName);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}